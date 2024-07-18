using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.VisualStyles;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Serilog;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.Core.UI.VisualStyles;

public class SociallyDistantVisualStyle : IVisualStyle
{
    private readonly IGameContext game;

    private readonly Color mainBackground                  = new Color(0x11, 0x13, 0x15);
    private readonly Color statusBarColor                  = new Color(0x01, 0x22, 0x37, 0xff);
    private readonly Color accentPrimary                   = new Color(0x13, 0x85, 0xC3, 0xff);
    private readonly Color accentEvil                      = new(0xDE, 0x17, 0x17);
    private readonly Color accentWarning                   = new Color(0xE0, 0x86, 0x17);
    private readonly Color accentSuccess                   = new Color(0x17, 0x82, 0x0E);
    private readonly Color accentCyberspace                = new Color(0x34, 0xB1, 0xFD);
    private readonly Color fieldBackground                 = new Color(0x25, 0x28, 0x2B);
    private readonly Color fieldStroke                     = new Color(0x60, 0x64, 0x67);
    private readonly Color tabInactiveBackgroundDefault    = new Color(0x44, 0x44, 0x44, 191);
    private readonly Color tabInactiveBorderDefault        = new Color(0x44, 0x44, 0x44);
    private readonly Color tabActiveBackgroundDefault      = new Color(0x16, 0x93, 0xD6, 190);
    private readonly Color tabActiveBorderDefault          = new Color(0x16, 0x93, 0xD6);
    private readonly Color sectionTextColor                = new(0x85, 0x85, 0x85);
    private readonly Color inputInactiveBorderColor        = new Color(0x54, 0x57, 0x5A);
    private readonly Color inputInactiveHoveredBorderColor = new Color(0x6F, 0x74, 0x77);
    private readonly Color inputActiveBorderColor          = new Color(0x19, 0xA1, 0xEA);
    private readonly Color inputActiveHoveredBorderColor   = new Color(0x80, 0xC3, 0xFD);
    private readonly Color inputInactiveBackground         = new Color(0x19, 0x1C, 0x1D);
    private readonly Color inputInactiveHoveredBackground  = new Color(0x2C, 0x2F, 0x32);
    private readonly Color inputInactivePressedBackground  = new Color(0x1F, 0x22, 0x25);
    private readonly Color inputActiveBackground           = new Color(0x13, 0x85, 0xC3);
    private readonly Color inputActiveHoveredBackground    = new Color(0x19, 0xA1, 0xEA);
    private readonly Color inputActivePressedBackground    = new Color(0x13, 0x85, 0xC3);
    private readonly Color buttonBackground                = new Color(0x44, 0x44, 0x44);
    private readonly Color buttonBorder                    = new Color(0x16, 0x93, 0xD6);
    private readonly Color buttonHoveredBackground         = new Color(0x0F, 0x73, 0xA9);
    private readonly Color buttonPressedBackground         = new Color(0x08, 0x53, 0x7B);
    private readonly Color selectionColor                  = new(0x08, 0x53, 0x7B);
    private readonly Color playerBubbleBackground          = new Color(0x08, 0x53, 0x7B);

    private Font        iconFont;
    private IFontFamily defaultFont = null!;
    private IFontFamily monospace   = null!;
    private Texture2D?  checkboxEmblem;

    public int SliderThickness => 18;
    public Point ToggleSize => new Point(20, 20);
    public Point SwitchSize => new Point(40, 22);
    public Font? IconFont => iconFont;
    public Padding DropdownButtonPadding { get; } = new Padding(1, 1, 29, 1);
    public Color SelectionColor => selectionColor;
    public Color TextSelectionBackground => selectionColor;
    public Color TextSelectionForeground => Color.White;

    public SociallyDistantVisualStyle(IGameContext game)
    {
        this.game = game;
    }

    private (Color, Color) GetChatBubbleColor(ChatBubbleColor color)
    {
        return color switch
        {
            ChatBubbleColor.Player => (playerBubbleBackground, accentPrimary),
            ChatBubbleColor.Npc => (fieldBackground, fieldStroke),
            _ => (default, default)
        };
    }
    
    internal void LoadContent()
    {
        checkboxEmblem = game.GameInstance.Content.Load<Texture2D>("/Core/UI/Textures/checkbox_emblem");
        
        iconFont = Font.FromTtfStream(
            game.GameInstance.Content.Load<Stream>("/Core/UI/Fonts/MaterialIcons-Regular.ttf"),
            256
        );

        defaultFont = LoadFont("/Core/UI/Fonts/Rajdhani");
        monospace = LoadFont("/Core/UI/Fonts/Monospace/JetBrainsMono");
    }

    private IFontFamily LoadFont(string basePath)
    {
        // We MUST load a regular font
        var regularFont = LoadFontStream(basePath + "-Regular.ttf");
        if (regularFont == null)
            throw new Exception(
                $"Could not load Regular variant for font {basePath}. Does {basePath}-Regular.tff exist in the Content File System?");
        
        var family = new FontFamily(regularFont);

        foreach (FontWeight weight in Enum.GetValues<FontWeight>())
        {
            Font? normal = LoadFontStream(basePath + $"-{weight}.ttf");
            Font? italic = LoadFontStream(basePath + $"-{weight}Italic.ttf");
            
            family.SetFont(weight, false, normal);
            family.SetFont(weight, true, italic);
        }

        return family;
    }

    private (int borderThickness, Color borderColor, Color backgroundColor) GetInputColor(
        bool isHovered,
        bool isPressed,
        bool isFocused,
        bool isChecked
    )
    {
        int thickness = isFocused
            ? 2
            : 1;

        Color background = default;
        Color foreground = default;

        if (isChecked)
        {
            foreground = (isHovered || isPressed)
                ? inputActiveHoveredBorderColor
                : inputActiveBorderColor;
            
            if (isHovered && isPressed)
            {
                background = inputActivePressedBackground;
            }
            else if (isHovered)
            {
                background = inputActiveHoveredBackground;
            }
            else
            {
                background = inputActiveBackground;
            }
        }
        else
        {
            foreground = (isHovered || isPressed)
                ? inputInactiveHoveredBorderColor
                : inputInactiveBorderColor;

            if (isHovered && isPressed)
            {
                background = inputInactivePressedBackground;
            }
            else if (isHovered)
            {
                background = inputInactiveHoveredBackground;
            }
            else
            {
                background = inputInactiveBackground;
            }
        }

        return (thickness, foreground, background);
    }
    
    private Font? LoadFontStream(string path)
    {
        try
        {
            using var stream = game.GameInstance.Content.Load<Stream>(path);
            return Font.FromTtfStream(stream, 16);
        }
        catch (Exception ex)
        {
            Log.Warning($"Failed to load GUI font: {path} - {ex.Message}");
            Log.Warning(ex.ToString());
            return null;
        }
    }
    
    public IFontFamily GetFont(PresetFontFamily familyPresetFont)
    {
        if (familyPresetFont == PresetFontFamily.Monospace)
            return monospace;
        
        return defaultFont;
    }

    private Color GetCommonColor(CommonColor commonColor)
    {
        return commonColor switch
        {
            CommonColor.Cyan => accentCyberspace,
            CommonColor.Yellow => accentWarning,
            CommonColor.Red => accentEvil,
            CommonColor.Green => accentSuccess,
            CommonColor.Blue => accentPrimary,
            _ => Color.White
        };
    }

    public int ScrollBarSize => 18;

    private void DrawInputField(InputField inputField, GeometryHelper geometryHelper)
    {
        var style = inputField.GetCustomProperty<InputFieldStyle>();

        var color = Color.White;

        switch (style)
        {
            case InputFieldStyle.Default:
            {
                var thickness = 1;
                
                geometryHelper.AddQuad(
                    new LayoutRect(
                        inputField.ContentArea.Left,
                        inputField.ContentArea.Bottom - thickness,
                        inputField.ContentArea.Width,
                        thickness
                    ), color
                );
                break;
            }
            case InputFieldStyle.Box:
            {
                var radius = 3;
                var thickness = 1;
                geometryHelper.AddRoundedRectangleOutline(
                    inputField.ContentArea,
                    thickness,
                    radius,
                    color
                );
                
                break;
            }
        }
    }

    private void DrawWindowTab(Widget widget, IWindowTab tab, GeometryHelper geometry)
    {
        var thickness = 1;
        var background = tabInactiveBackgroundDefault;
        var border = tabInactiveBorderDefault;

        if (tab.Active)
        {
            background = tabActiveBackgroundDefault;
            border = tabActiveBorderDefault;
        }

        geometry.AddRoundedRectangle(
            widget.ContentArea,
            3,
            3,
            0,
            0,
            background
        );
        
        geometry.AddRoundedRectangleOutline(
            widget.ContentArea,
            thickness,
            3,
            3,
            0,
            0,
            border
        );
    }

    private void DrawDecorativeBlock(DecorativeBlock widget, GeometryHelper geometry)
    {
        var color = (widget.BoxColor ?? GetCommonColor(widget.GetCustomProperty<CommonColor>()));

        if (widget.Opaque)
        {
            geometry.AddRoundedRectangle(
                widget.ContentArea,
                3,
                color
            );
        }
        else
        {
            geometry.AddRoundedRectangle(
                widget.ContentArea,
                3,
                color * 0.5f
            );

            geometry.AddRoundedRectangleOutline(
                widget.ContentArea,
                2,
                3,
                color
            );


        }
    }

    public Color GetTextColor(Widget? widget = null)
    {
        if (widget != null)
        {
            switch (widget.GetCustomProperty<WidgetForegrounds>())
            {
                case WidgetForegrounds.Common:
                {
                    var commonColor = widget.GetCustomProperty<CommonColor>();
                    return GetCommonColor(commonColor);
                }
                case WidgetForegrounds.SectionTitle:
                    return sectionTextColor;
            }
        }

        return Color.White;
    }

    private void DrawTextButton(TextButton widget, GeometryHelper geometry)
    {
        var focused = widget.IsFocused;
        var hovered = widget.IsHovered;
        var pressed = widget.IsPressed;

        var background = buttonBackground;
        var border = buttonBorder;

        if (hovered && pressed)
        {
            background = buttonPressedBackground;
        }
        else if (hovered)
        {
            background = buttonHoveredBackground;
        }

        geometry.AddRoundedRectangle(widget.ContentArea, 3, background);

        if (hovered || pressed || focused)
        {
            geometry.AddRoundedRectangleOutline(widget.ContentArea, 1, 3, border);
        }
    }

    private void DrawListItem(ListItem listItem, GeometryHelper geometry)
    {
        var color = selectionColor;

        if (listItem.IsActive)
        {
            geometry.AddRoundedRectangle(listItem.ContentArea, 3, color);
        }
        else if (listItem.IsHovered)
        {
            geometry.AddRoundedRectangle(listItem.ContentArea, 3, color * 0.5f);
        }
    }

    private void DrawEmblem(Emblem emblem, GeometryHelper geometry)
    {
        geometry.AddRoundedRectangle(emblem.ContentArea, 3, mainBackground);
        geometry.AddRoundedRectangleOutline(emblem.ContentArea, 1, 3, GetCommonColor(emblem.Color));
    }
    
    public void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper)
    {
        if (widget is InputField inputField)
            DrawInputField(inputField, geometryHelper);
        else if (widget is ListItem listItem)
            DrawListItem(listItem, geometryHelper);
        else if (widget is TextButton textButton)
            DrawTextButton(textButton, geometryHelper);
        else if (widget is IWindowTab tab)
            DrawWindowTab(widget, tab, geometryHelper);
        else if (widget is DecorativeBlock box)
            DrawDecorativeBlock(box, geometryHelper);
        else if (widget is Emblem emblem)
            DrawEmblem(emblem, geometryHelper);
        else
        {
            WidgetBackgrounds background = widget.GetCustomProperty<WidgetBackgrounds>();

            switch (background)
            {
                case WidgetBackgrounds.StatusBar:
                    DrawStatusBar(widget, geometryHelper);
                    break;
                case WidgetBackgrounds.Common:
                    geometryHelper.AddQuad(widget.ContentArea, GetCommonColor(widget.GetCustomProperty<CommonColor>()));
                    break;
                case WidgetBackgrounds.Overlay:
                    geometryHelper.AddQuad(widget.ContentArea, mainBackground * 0.67f);
                    break;
                case WidgetBackgrounds.Dock:
                    DrawDock(widget, geometryHelper);
                    break;
                case WidgetBackgrounds.WindowClient:
                    geometryHelper.AddQuad(widget.ContentArea, mainBackground);
                    break;
                case WidgetBackgrounds.WindowBorder:
                    geometryHelper.AddQuadOutline(widget.ContentArea, 1,
                        GetCommonColor(widget.GetCustomProperty<CommonColor>()));
                    break;
                case WidgetBackgrounds.FormField:
                    geometryHelper.AddRoundedRectangle(widget.ContentArea, 3, fieldBackground);
                    geometryHelper.AddRoundedRectangleOutline(widget.ContentArea, 1, 3, fieldStroke);
                    break;
                case WidgetBackgrounds.ChatBubble:
                {
                    (Color bg, Color stroke) = GetChatBubbleColor(widget.GetCustomProperty<ChatBubbleColor>());

                    geometryHelper.AddRoundedRectangle(widget.ContentArea, 12, bg);
                    geometryHelper.AddRoundedRectangleOutline(widget.ContentArea, 2, 12, stroke);
                    break;
                }
                case WidgetBackgrounds.CompletionList:
                {
                    geometryHelper.AddRoundedRectangle(widget.ContentArea, 3, mainBackground);
                    geometryHelper.AddRoundedRectangleOutline(widget.ContentArea, 1, 3, fieldStroke);
                    
                    break;
                }
            }
        }
    }

    private void DrawStatusBar(Widget widget, GeometryHelper geometry)
    {
        geometry.AddQuad(widget.ContentArea, statusBarColor);
    }

    private void DrawDock(Widget widget, GeometryHelper geometry)
    {
        geometry.AddRoundedRectangle(widget.ContentArea, 3, mainBackground);
    }
    
    public void DrawScrollBar(Widget widget, GeometryHelper geometry, LayoutRect scrollBarArea, int scrollOffset,
        int scrollViewHeight)
    {
        int barHeight = scrollBarArea.Height / scrollViewHeight * scrollBarArea.Height;
        int barOffset = (scrollOffset / scrollViewHeight) * scrollBarArea.Height;

        geometry.AddQuad(scrollBarArea, Color.Gray);
        
        geometry.AddQuad(
            new LayoutRect(
                scrollBarArea.Left,
                scrollBarArea.Top + barOffset,
                scrollBarArea.Width,
                barHeight
            ),
            Color.White
        );
    }

    public void DrawToggle(
        Toggle toggle,
        GeometryHelper geometry,
        LayoutRect rect,
        bool isHovered,
        bool isPressed,
        bool isFocused,
        bool isChecked
    )
    {
        (float borderThickness, Color borderColor, Color backgroundColor) = GetInputColor(isHovered, isPressed, isFocused, isChecked);

        geometry.AddRoundedRectangle(rect, 3, backgroundColor);
        geometry.AddRoundedRectangleOutline(rect, borderThickness, 3, borderColor);

        if (isChecked && checkboxEmblem != null)
        {
            var emblemRect = new LayoutRect(rect.Left + ((rect.Width - checkboxEmblem.Width) / 2), rect.Top + ((rect.Height - checkboxEmblem.Height) / 2), checkboxEmblem.Width, checkboxEmblem.Height);
            geometry.AddQuad(emblemRect, Color.White, checkboxEmblem);
        }
    }

    public void DrawToggleSwitch(
        Toggle toggle,
        GeometryHelper geometry,
        LayoutRect rect,
        bool isHovered,
        bool isPressed,
        bool isFocused,
        bool isChecked
    )
    {
        (int borderThickness, Color borderColor, Color backgroundColor) = GetInputColor(isHovered, isPressed, isFocused, isChecked);

        geometry.AddRoundedRectangle(rect, rect.Height/2, backgroundColor);
        geometry.AddRoundedRectangleOutline(rect, borderThickness, rect.Height/2, borderColor);

        const int nubOffset = 4;
        
        Point nubSize = new Point(16, 16);

        LayoutRect nubRect = new LayoutRect(!isChecked
            ? rect.Left + nubOffset
            : rect.Right - nubSize.X - nubOffset, rect.Top + ((rect.Height - nubSize.Y) / 2), nubSize.X, nubSize.Y);

        geometry.AddRoundedRectangle(nubRect, nubSize.X / 2, Color.White);
    }

    public void DrawDropdownItemsBackground(GeometryHelper geometry, LayoutRect rect)
    {
        geometry.AddQuad(rect, mainBackground);
        geometry.AddQuadOutline(rect, 1, fieldStroke);
    }

    public void DrawDropdownItemBackground(
        Widget widget,
        GeometryHelper geometry,
        bool hovered,
        bool pressed,
        bool selected
    )
    {
        if (selected)
            geometry.AddQuad(widget.ContentArea, accentPrimary);

        if (hovered)
        {
            geometry.AddQuad(widget.ContentArea, Color.White * 0.25f);
        }

        if (pressed)
        {
            geometry.AddQuad(widget.ContentArea, Color.Black * 0.25f);
        }
    }

    public void DrawSlider(
        Slider widget,
        GeometryHelper geometry,
        bool isHovered,
        bool isPressed,
        bool isVertical,
        float value
    )
    {
        (int thickness, Color border, Color background) = GetInputColor(isHovered, isPressed, false, true);


        int halfThickness = SliderThickness / 2;
        int eighthThickness = halfThickness / 2;

        int nubOffsetX = 0;
        int nubOffsetY = 0;
        
        if (isVertical)
        {
            int left = widget.ContentArea.Top + halfThickness;
            int right = widget.ContentArea.Bottom - halfThickness;
            int width = right - left;
            int nubCenter = (int) MathHelper.Lerp(right, left, value);
            int fillWidth = nubCenter - left;
            int top = widget.ContentArea.Left + ((widget.ContentArea.Width - eighthThickness) / 2);
            
            nubOffsetX = widget.ContentArea.Left + (widget.ContentArea.Width - SliderThickness) / 2;
            nubOffsetY = nubCenter - halfThickness;
            
            geometry.AddRoundedRectangle(new LayoutRect(top, left,      eighthThickness, width),     eighthThickness/2,   inputInactiveBackground);
            geometry.AddRoundedRectangle(new LayoutRect(top, nubCenter, eighthThickness, fillWidth), eighthThickness / 2, accentPrimary);
        }
        else 
        {
            int left = widget.ContentArea.Left + halfThickness;
            int right = widget.ContentArea.Right - halfThickness;
            int width = right - left;
            int nubCenter = (int) MathHelper.Lerp(left, right, value);
            int fillWidth = nubCenter - left;
            int top = widget.ContentArea.Top + ((widget.ContentArea.Height - eighthThickness) / 2);
            
            nubOffsetX = nubCenter - halfThickness;
            nubOffsetY = widget.ContentArea.Top + (widget.ContentArea.Height - SliderThickness) / 2;
            
            geometry.AddRoundedRectangle(new LayoutRect(left, top, width,     eighthThickness), eighthThickness/2,   inputInactiveBackground);
            geometry.AddRoundedRectangle(new LayoutRect(left, top, fillWidth, eighthThickness), eighthThickness / 2, accentPrimary);
        }
        
        geometry.AddRoundedRectangle(new LayoutRect(nubOffsetX,        nubOffsetY, SliderThickness, SliderThickness), halfThickness, background);
            
        geometry.AddRoundedRectangleOutline(new LayoutRect(nubOffsetX, nubOffsetY, SliderThickness, SliderThickness), thickness, halfThickness, border);
    }
}

public enum InputFieldStyle
{
    Default,
    Box
}

public enum WidgetBackgrounds
{
    None,
    Common,
    Overlay,
    StatusBar,
    Dock,
    WindowClient,
    WindowBorder,
    FormField,
    ChatBubble,
    CompletionList
}

public enum ChatBubbleColor
{
    None,
    Player,
    Npc
}

public enum WidgetForegrounds
{
    Default,
    Common,
    SectionTitle
}