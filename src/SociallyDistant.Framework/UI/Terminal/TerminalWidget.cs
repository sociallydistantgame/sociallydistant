using AcidicGUI;
using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Serilog;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.Network.MessageTransport;
using SociallyDistant.Core.UI.Effects;

namespace SociallyDistant.Core.UI.Terminal;

public class TerminalWidget : Widget,
    IDrawableScreen,
    IClipboard,
    ITerminalSounds,
    IUpdateHandler,
    IGainFocusHandler,
    ILoseFocusHandler,
    IMouseDownHandler,
    IKeyDownHandler,
    IKeyCharHandler,
    IKeyUpHandler
{
    private readonly WorkQueue                    workQueue         = new();
    private readonly WorkQueue                    emulatorWorkQueue = new WorkQueue();
    private readonly MultiCancellationTokenSource tokenSource       = new MultiCancellationTokenSource();
    private readonly TerminalColorPalette         defaultPalette    = TerminalColorPalette.Default;
    private          int                          clickCount;
    private          int                          mainThreadId;
    private          float                        characterWidth;
    private          float                        lineHeight;
    private          int                          rowCount;
    private          int                          columnCount;
    private readonly SimpleTerminal               simpleTerminal;
    private          TerminalOptions              ptyOptions = new TerminalOptions();
    private          PseudoTerminal               master;
    private          PseudoTerminal               slave;
    private          ITextConsole                 console;
    private          Thread?                      emulatorThread;
    private          ManualResetEvent             emulatorShutdownCompleted = new ManualResetEvent(false);
    private volatile bool                         emulatorEnabled           = false;
    private          float                        clickTime                 = 0;
    private          float                        clickDoubleTime           = 0;
    private          Task?                        resizeTask;
    private          int                          desiredColumnCount = 0;
    private          int                          desiredRowCount    = 0;
    private          RenderCell[]                 cells              = Array.Empty<RenderCell>();
    private          bool                         focused;
    private          int                          currentCursorX;
    private          int                          currentCursorY;
    private          TerminalColorPalette?        paletteOverride;
    private          float                        backgroundOpacity = 1;
    private          bool                         controlDown;
    private          bool                         altDown;
    private          bool                         shiftDown;
    private          bool                         rightControlDown;
    private          bool                         rightAltDown;
    private          bool                         rightShiftDown;

    public ITextConsole Console => console;

    public TerminalColorPalette? ColorPalette
    {
        get => paletteOverride;
        set
        {
            paletteOverride = value;
            UpdateColors();
        }
    }

    public float BackgroundOpacity
    {
        get => backgroundOpacity;
        set
        {
            backgroundOpacity = MathHelper.Clamp(value, 0, 1);
            InvalidateGeometry();
        }

    }
    
    public TerminalWidget()
    {
        this.RenderEffect = BackgroundBlurWidgetEffect.GetEffect(Application.Instance.Context);
        
        workQueue.MaximumWorkPerUpdate = 1;
        mainThreadId = Thread.CurrentThread.ManagedThreadId;

        simpleTerminal = new SimpleTerminal(this, this, this, 0, 0);
        
        // Enforce CRLF
        this.ptyOptions.LFlag = 0;

        // Control codes
        this.ptyOptions.C_cc[PtyConstants.VERASE] = (byte)'\b';

        PseudoTerminal.CreatePair(out this.master, out this.slave, this.ptyOptions);

        this.simpleTerminal.SetTty(new SociallyDistantTty(this.master));
        this.console = new SimpleTerminalSession(this.simpleTerminal, this.slave, new RepeatableCancellationToken(tokenSource));
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        var fontFamily = this.GetFont(PresetFontFamily.Monospace);

        Vector2 charSize = fontFamily.Measure("#");

        this.characterWidth = charSize.X;
        this.lineHeight = fontFamily.GetLineHeight();

        int availableColumns = (int)MathF.Floor(availableSize.X / characterWidth);
        int availableRows = (int)MathF.Floor(availableSize.Y / lineHeight);

        float x = availableSize.X;
        float y = availableSize.Y;

        if (desiredColumnCount > 0)
            x = desiredColumnCount * characterWidth;
        if (desiredRowCount > 0)
            y = desiredRowCount * lineHeight;

        return new Vector2(x, y);
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        int availableColumns = (int)MathF.Floor(availableSpace.Width / characterWidth);
        int availableRows = (int)MathF.Floor(availableSpace.Height / lineHeight);

        int actualRowCount = Math.Min(availableRows,       desiredRowCount);
        int actualColumnCount = Math.Min(availableColumns, desiredColumnCount);

        if (desiredColumnCount == 0)
            actualColumnCount = availableColumns;
        else 
            actualColumnCount = Math.Max(actualColumnCount,       1);
        
        if (desiredRowCount == 0)
            actualRowCount = availableRows;
        else 
            actualRowCount = Math.Max(actualRowCount,       1);

        rowCount = actualRowCount;
        columnCount = actualColumnCount;

        if (simpleTerminal.Rows == 0 || simpleTerminal.Columns == 0)
        {
            Array.Resize(ref cells, actualColumnCount * actualRowCount);
            simpleTerminal.TerminalNew(actualColumnCount, actualRowCount);
        }
        else if (simpleTerminal.Rows != actualRowCount || simpleTerminal.Columns != actualColumnCount)
        {
            Array.Resize(ref cells, actualColumnCount * actualRowCount);
            simpleTerminal.Resize(actualColumnCount, actualRowCount);
        }
    }
    
    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        TerminalColorPalette palette = paletteOverride ?? defaultPalette;
        IFontFamily fontFamily = GetFont(PresetFontFamily.Monospace);
 
        geometry.AddQuad(ContentArea, palette.DefaultBackground * backgroundOpacity);
        
        for (var i = 0; i < cells.Length; i++)
        {
            ref RenderCell cell = ref cells[i];
            
            int column = i % columnCount;
            int row = i / columnCount;

            float x = ContentArea.Left + column * characterWidth;
            float y = ContentArea.Top + row * lineHeight;

            var fg = cell.Foreground;
            var bg = cell.Background;
            if (column == currentCursorX && row == currentCursorY && focused)
            {
                (fg, bg) = (bg, fg);
                fg.A = 0xff;
            }

            bool italic = cell.GlyphData.mode.HasFlag(GlyphAttribute.ATTR_ITALIC);
            FontWeight weight = FontWeight.Normal;

            if (cell.GlyphData.mode.HasFlag(GlyphAttribute.ATTR_BOLD))
            {
                weight = cell.GlyphData.mode.HasFlag(GlyphAttribute.ATTR_FAINT)
                    ? FontWeight.Medium
                    : FontWeight.Bold;
            }
            else
            {
                weight = cell.GlyphData.mode.HasFlag(GlyphAttribute.ATTR_FAINT)
                    ? FontWeight.Light
                    : FontWeight.Normal;
            }
            
            geometry.AddQuad(new LayoutRect(x, y, characterWidth, lineHeight), bg);

            if (cell.GlyphData.mode.HasFlag(GlyphAttribute.ATTR_INVISIBLE) || fg.A==0 || cell.GlyphData.character==' ')
                continue;
            
            fontFamily.Draw(geometry, new Vector2(x, y), fg, cell.GlyphData.character.ToString(), null, weight, italic);
        }
    }

    private Color GetColor(int index, Color? user)
    {
        TerminalColorPalette palette = paletteOverride ?? defaultPalette;
        
        if (user.HasValue)
            return user.Value;

        if (index < 16)
            return palette.GetConsoleColor((ConsoleColor)index);

        if (index == SimpleTerminal.defaultbg)
            return palette.DefaultBackground;

        if (index == SimpleTerminal.defaultfg)
            return palette.DefaultForeground;

        return Color.Transparent;
    }
    
    private void UpdateColor(ref RenderCell cell)
    {
        int fgIndex = cell.GlyphData.fg;
        int bgIndex = cell.GlyphData.bg;

        var useTransparent = bgIndex == SimpleTerminal.defaultbg;
        
        if (cell.GlyphData.mode.HasFlag(GlyphAttribute.ATTR_REVERSE))
        {
            useTransparent = false;
            (fgIndex, bgIndex) = (bgIndex, fgIndex);
        }
        
        Color bg = GetColor(bgIndex, cell.GlyphData.bgRgb);
        Color fg = GetColor(fgIndex, cell.GlyphData.fgRgb);

        cell.Foreground = fg;
        cell.Background = useTransparent
            ? Color.Transparent
            : bg;
    }
    
    private void UpdateColors()
    {
        for (var i = 0; i < cells.Length; i++)
        {
            ref RenderCell cell = ref cells[i];
            UpdateColor(ref cell);
        }
    }
    
    public void DrawLine(
        SimpleTerminal term,
        ref Glyph[] glyphs,
        int x1,
        int y,
        int x2
    )
    {
        for (int x = x1; x < x2; x++)
        {
            var i = y * columnCount + x;
            cells[i].GlyphData = glyphs[x];

            UpdateColor(ref cells[i]);
        }

        InvalidateGeometry();
    }

    public void Resize(int columns, int rows)
    {
        InvalidateGeometry();
    }

    public void Bell()
    {
    }

    public void ScreenPointToCell(
        SimpleTerminal term,
        float x,
        float y,
        out int column,
        out int row
    )
    {
        column = (int)Math.Round((x - ContentArea.Left) / characterWidth);
        row = (int)Math.Round((y - ContentArea.Top) / lineHeight);
    }

    public void AfterRender(int cursorX, int cursorY)
    {
        if (currentCursorX != cursorX || currentCursorY != cursorY)
            InvalidateGeometry();

        currentCursorX = cursorX;
        currentCursorY = cursorY;
    }

    public void SetFocus(bool isFocused)
    {
        focused = isFocused;
        InvalidateGeometry();
    }

    public string GetText()
    {
        Log.Warning("Clipboard not supported yet.");
        return string.Empty;
    }

    public void SetText(string text)
    {
        Log.Warning("Clipboard not supported yet.");
    }

    public void PlayTypingSound()
    {
    }

    public void Update(float deltaTime)
    {
        simpleTerminal.Update(deltaTime);
    }

    private struct RenderCell
    {
        public Color   Background;
        public Color   Foreground;
        public Glyph   GlyphData;
        public Vector2 Position;
    }

    public void OnFocusGained(FocusEvent e)
    {
        simpleTerminal.SetFocus(true);
    }

    public void OnFocusLost(FocusEvent e)
    {
        simpleTerminal.SetFocus(false);
    }

    public void OnMouseDown(MouseButtonEvent e)
    {
        // TODO: Actually send mouse events to simpleTerminal.
        e.RequestFocus();
    }

    public void OnKeyChar(KeyCharEvent e)
    {
        // These keys are handled by KeyDown
        if (e.Key == Keys.Back || e.Key == Keys.Tab || e.Key == Keys.Delete||e.Key == Keys.Enter)
            return;
        
        simpleTerminal.Input.Char(e.Character);
    }

    public void OnKeyDown(KeyEvent e)
    {
        if (e.Key == Keys.LeftControl)
            controlDown = true;

        if (e.Key == Keys.RightControl)
            rightControlDown = true;

        if (e.Key == Keys.LeftShift)
            shiftDown = true;

        if (e.Key == Keys.RightShift)
            rightShiftDown = true;

        if (e.Key == Keys.LeftAlt)
            altDown = true;

        if (e.Key == Keys.RightAlt)
            rightAltDown = true;
        
        
        simpleTerminal.Input.Raw(e.Key, controlDown || rightControlDown, altDown || rightAltDown, shiftDown || rightShiftDown);
    }

    public void OnKeyUp(KeyEvent e)
    {
        if (e.Key == Keys.LeftControl)
            controlDown = false;

        if (e.Key == Keys.RightControl)
            rightControlDown = false;

        if (e.Key == Keys.LeftShift)
            shiftDown = false;

        if (e.Key == Keys.RightShift)
            rightShiftDown = false;

        if (e.Key == Keys.LeftAlt)
            altDown = false;

        if (e.Key == Keys.RightAlt)
            rightAltDown = false;

    }
}