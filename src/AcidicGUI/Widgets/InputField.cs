using System.Text;
using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AcidicGUI.Widgets;

public sealed class InputField : 
    Widget,
    IMouseClickHandler,
    IGainFocusHandler,
    ILoseFocusHandler,
    IMouseEnterHandler,
    IMouseLeaveHandler,
    IMouseMoveHandler,
    IMouseScrollHandler,
    IKeyDownHandler,
    IKeyUpHandler,
    IKeyCharHandler
{
    private readonly StringBuilder currentValue = new();
    private readonly TextWidget display = new();
    private readonly int displayPadding = 3;

    private bool   focused;
    private string placeholder = string.Empty;
    private int    caretIndex  = 0;
    private bool   multiline;
    private int    selectionOffset;
    private bool   selectAllOnFocus = true;
    private bool   submitOnEnter;

    public bool SubmitOnEnter
    {
        get => submitOnEnter;
        set => submitOnEnter = value;
    }
    
    public bool SelectAllOnFocus
    {
        get => selectAllOnFocus;
        set => selectAllOnFocus = value;
    }
    
    public string Value
    {
        get => currentValue.ToString();
        set => SetValueInternal(value);
    }

    public string Placeholder
    {
        get => placeholder;
        set
        {
            placeholder = value;
            InvalidateLayout();
        }
    }
    
    public bool WordWrapped
    {
        get => display.WordWrapping;
        set => display.WordWrapping = value;
    }

    public bool MultiLine
    {
        get => multiline;
        set => multiline = value;
    }

    public event Action<string>? OnSubmit;
    public event Action<string>? OnValueChanged;
    
    public InputField()
    {
        Children.Add(display);
        ClippingMode = ClippingMode.Clip;
        
        display.ShowMarkup = true;
    }

    protected override Point GetContentSize(Point availableSize)
    {
        var baseSize = base.GetContentSize(availableSize);

        Point placeholderSize = display.Font.GetFont(display).Measure(placeholder);

        baseSize.X = Math.Max(baseSize.X, placeholderSize.X);
        baseSize.Y = Math.Max(baseSize.Y, placeholderSize.Y);
        
        return new Point(baseSize.X + (displayPadding * 2), baseSize.Y + (displayPadding * 2));
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        // Pass 1: Normal layout. This will rebuild text elements in the display.
        base.ArrangeChildren(context, new LayoutRect(
            availableSpace.Left + displayPadding,
            availableSpace.Top + displayPadding,
            availableSpace.Width - (displayPadding * 2),
            availableSpace.Height - (displayPadding * 2)
        ));
        
        // Pass 2: If we're focused, make sure the caret is visible at all times.
        if (focused)
        {
            LayoutRect caretRect = display.GetPositionOfCharacter(caretIndex);

            int left = availableSpace.Left + displayPadding;
            int top = availableSpace.Top + displayPadding;
            int width = availableSpace.Width - (displayPadding * 2);
            int height = availableSpace.Height - (displayPadding * 2);

            if (caretRect.Left < left)
            {
                int distance = left - caretRect.Left;
                left -= distance;
            }
            else if (caretRect.Right > left + width)
            {
                int distance = caretRect.Right - (left + width);
                left -= distance;
            }
            
            if (caretRect.Top < top)
            {
                int distance = top - caretRect.Top;
                top -= distance;
            }
            else if (caretRect.Bottom > top + height)
            {
                int distance = caretRect.Bottom - (top + height);
                top -= distance;
            }

            display.InvalidateOwnLayout();
            display.UpdateLayout(context, new LayoutRect(
                left,
                top,
                width,
                height
            ));
        }
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        base.RebuildGeometry(geometry);

        if (focused)
        {
            LayoutRect caretRect = display.GetPositionOfCharacter(caretIndex);
            geometry.AddQuad(
                new LayoutRect(
                    caretRect.Left,
                    caretRect.Top,
                    1,
                    caretRect.Height
                ),
                Color.White
            );
        }
        else if (!string.IsNullOrWhiteSpace(placeholder))
        {
            var fontInstance = display.Font.GetFont(display);

            fontInstance.Draw(
                geometry,
                new Vector2(
                    ContentArea.Left + displayPadding,
                    ContentArea.Top + displayPadding
                ),
                Color.White * 0.5f,
                placeholder
            );
        }
    }

    private void HandleEnter(bool shiftPressed)
    {
        if (multiline && (!submitOnEnter || shiftPressed))
        {
            InsertText('\n');
            return;
        }
        
        OnSubmit?.Invoke(currentValue.ToString());
    }
    
    private void SetValueInternal(string value)
    {
        this.currentValue.Length = 0;
        this.currentValue.Append(value);
        caretIndex = Math.Clamp(caretIndex, 0, currentValue.Length);
        caretIndex = currentValue.Length;
        selectionOffset = 0;

        UpdateDisplay();
        OnValueChanged?.Invoke(currentValue.ToString());
    }

    private void Delete(int index, int count)
    {
        if (selectionOffset != 0)
        {
            DeleteSelection();
            return;
        }
        
        if (index < 0)
            return;

        if (index + count > currentValue.Length)
            return;

        int length = currentValue.Length;
        currentValue.Remove(index, count);
        int deleted = length - currentValue.Length;

        if (caretIndex > index)
        {
            if (caretIndex <= index + count)
                caretIndex = index;
            else
                caretIndex -= deleted;
        }

        UpdateDisplay();
        
        OnValueChanged?.Invoke(currentValue.ToString());
    }
    
    private void InsertText(char character)
    {
        DeleteSelection();
        
        if (caretIndex == currentValue.Length)
            currentValue.Append(character);
        else currentValue.Insert(caretIndex, character);
        caretIndex++;
        UpdateDisplay();
        
        OnValueChanged?.Invoke(currentValue.ToString());
    }
    
    private void UpdateDisplay()
    {
        int actualSelectionStart = Math.Min(caretIndex, caretIndex + selectionOffset);
        int actualSelectionEnd = Math.Max(caretIndex, caretIndex + selectionOffset);
        int selectionLength = actualSelectionEnd - actualSelectionStart;
        
        this.display.Text = currentValue.ToString();

        if (selectionLength > 0)
            display.SetSelection(actualSelectionStart, selectionLength);
        
        InvalidateLayout();
        
        
    }
    
    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.RequestFocus();
    }

    public void OnFocusGained(FocusEvent e)
    {
        if (selectAllOnFocus)
            SelectAll();
        
        focused = true;
        InvalidateGeometry();
    }

    public void OnFocusLost(FocusEvent e)
    {
        DeselectAll();
        
        focused = false;
        InvalidateGeometry();
    }

    public void OnMouseEnter(MouseMoveEvent e)
    {
    }

    public void OnMouseLeave(MouseMoveEvent e)
    {
    }

    public void OnMouseMove(MouseMoveEvent e)
    {
    }

    public void OnMouseScroll(MouseScrollEvent e)
    {
    }

    private void MoveToLineStart()
    {
        DeselectAll();
        
        var line = display.GetLineAtIndex(caretIndex);
        caretIndex = display.GetLineStart(line);
        InvalidateLayout();
    }

    private void MoveToLineEnd()
    {
        DeselectAll();
        
        var line = display.GetLineAtIndex(caretIndex);
        caretIndex = display.GetLineStart(line) + display.GetLineLength(line);
        InvalidateLayout();
    }

    private void MoveUp(int lines)
    {
        DeselectAll();
        
        int currentLine = display.GetLineAtIndex(caretIndex);
        int lineCount = display.GetLineCount();
        int nextLine = Math.Clamp(currentLine - lines, 0, lineCount - 1);

        if (currentLine == nextLine)
        {
            if (!multiline)
                MoveToLineStart();
            
            return;
        }

        int currentLineStart = display.GetLineStart(currentLine);
        int indexInLine = caretIndex - currentLineStart;

        int nextLineStart = display.GetLineStart(nextLine);
        int nextLineCount = display.GetLineLength(nextLineStart);

        caretIndex = nextLineStart + Math.Min(indexInLine, nextLineCount);
        InvalidateLayout();
    }

    private void SelectAll()
    {
        caretIndex = 0;
        selectionOffset = currentValue.Length;
        UpdateDisplay();
    }
    
    private void DeselectAll()
    {
        if (selectionOffset != 0)
        {
            selectionOffset = 0;
            UpdateDisplay();
        }
    }
    
    private void SelectionMove(int amount)
    {
        int newOffset = selectionOffset + amount;
        int caretAdjusted = caretIndex + newOffset;

        if (caretAdjusted > currentValue.Length)
            newOffset = currentValue.Length - caretIndex;
        else if (caretAdjusted < 0)
            newOffset = -caretIndex;

        selectionOffset = newOffset;
        UpdateDisplay();
    }
    
    private void MoveRight(int characters)
    {
        if (selectionOffset != 0)
        {
            int caret = Math.Max(caretIndex, caretIndex + selectionOffset);
            DeselectAll();
            caretIndex = caret;
            InvalidateLayout();
            return;
        }
        
        if (caretIndex < currentValue.Length)
        {
            caretIndex++;
            InvalidateLayout();
        }
    }
    
    private void MoveLeft(int characters)
    {
        if (selectionOffset != 0)
        {
            int caret = Math.Min(caretIndex, caretIndex + selectionOffset);
            DeselectAll();
            caretIndex = caret;
            InvalidateLayout();
            return;
        }
        
        if (caretIndex > 0)
        {
            caretIndex--;
            InvalidateLayout();
        }
    }
    
    private void MoveDown(int lines)
    {
        DeselectAll();
        
        int currentLine = display.GetLineAtIndex(caretIndex);
        int lineCount = display.GetLineCount();
        int nextLine = Math.Clamp(currentLine + lines, 0, lineCount - 1);

        if (currentLine == nextLine)
        {
            if (!multiline)
                MoveToLineStart();
            
            return;
        }

        int currentLineStart = display.GetLineStart(currentLine);
        int indexInLine = caretIndex - currentLineStart;

        int nextLineStart = display.GetLineStart(nextLine);
        int nextLineCount = display.GetLineLength(nextLineStart);

        caretIndex = nextLineStart + Math.Min(indexInLine, nextLineCount);
        InvalidateLayout();
    }

    private void DeleteSelection()
    {
        int selectionStart = Math.Min(caretIndex, caretIndex + selectionOffset);
        int selectionEnd = Math.Max(caretIndex, caretIndex + selectionOffset);
        int selectionCount = selectionEnd - selectionStart;

        if (selectionCount == 0)
            return;

        caretIndex = selectionStart;
        currentValue.Remove(selectionStart, selectionCount);
        selectionOffset = 0;
        UpdateDisplay();
        
        OnValueChanged?.Invoke(currentValue.ToString());
    }
    
    public void OnKeyDown(KeyEvent e)
    {
        var selectionMode = e.Modifiers.HasFlag(ModifierKeys.Shift);
        
        switch (e.Key)
        {
            case Keys.Home:
            {
                MoveToLineStart();
                break;
            }
            case Keys.End:
            {
                MoveToLineEnd();
                break;
            }
            case Keys.Up:
            {
                MoveUp(1);
                break;
            }
            case Keys.Down:
            {
                MoveDown(1);
                break;
            }
            case Keys.Left:
            {
                if (selectionMode)
                    SelectionMove(-1);
                else 
                    MoveLeft(1);
                break;
            }
            case Keys.Right:
            {
                if (selectionMode)
                    SelectionMove(1);
                else 
                    MoveRight(1);
                break;
            }
        }
    }

    public void OnKeyUp(KeyEvent e)
    {
    }

    public void OnKeyChar(KeyCharEvent e)
    {
        e.Handle();

        if (e.Key == Keys.Escape)
        {
            DeselectAll();
            caretIndex = 0;
            GuiManager?.SetFocusedWidget(this.Parent);
            e.Handle();
            return;
        }
        
        if (e.Key == Keys.Enter)
        {
            HandleEnter(e.Modifiers.HasFlag(ModifierKeys.Shift));
            return;
        }

        // TODO: Tabs can't be rendered.
        if (e.Key == Keys.Tab)
            return;
        
        switch (e.Character)
        {
            case '\b':
                Delete(caretIndex - 1, 1);
                break;
            case '\x7f':
                Delete(caretIndex, 1);
                break;
            default:
                InsertText(e.Character);
                break;
        }
    }
}