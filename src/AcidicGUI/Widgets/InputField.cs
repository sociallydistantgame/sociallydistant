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
    private readonly float displayPadding = 3;

    private bool focused;
    private string placeholder = string.Empty;
    private int caretIndex = 0;
    private bool multiline;

    public string Value
    {
        get => currentValue.ToString();
        set => SetValueInternal(value);
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
    
    public InputField()
    {
        Children.Add(display);
        ClippingMode = ClippingMode.Clip;
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        var baseSize = base.GetContentSize(availableSize);

        Vector2 placeholderSize = display.Font.GetFont(display).Measure(placeholder);

        baseSize.X = Math.Max(baseSize.X, placeholderSize.X);
        baseSize.Y = Math.Max(baseSize.Y, placeholderSize.Y);
        
        return new Vector2(baseSize.X + (displayPadding * 2), baseSize.Y + (displayPadding * 2));
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

            float left = availableSpace.Left + displayPadding;
            float top = availableSpace.Top + displayPadding;
            float width = availableSpace.Width - (displayPadding * 2);
            float height = availableSpace.Height - (displayPadding * 2);

            if (caretRect.Left < left)
            {
                float distance = left - caretRect.Left;
                left -= distance;
            }
            else if (caretRect.Right > left + width)
            {
                float distance = caretRect.Right - (left + width);
                left -= distance;
            }
            
            if (caretRect.Top < top)
            {
                float distance = top - caretRect.Top;
                top -= distance;
            }
            else if (caretRect.Bottom > top + height)
            {
                float distance = caretRect.Bottom - (top + height);
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

    private void HandleEnter()
    {
        if (multiline)
        {
            InsertText('\n');
            return;
        }
    }
    
    private void SetValueInternal(string value)
    {
        this.currentValue.Length = 0;
        this.currentValue.Append(value);
        caretIndex = Math.Clamp(caretIndex, 0, currentValue.Length);

        UpdateDisplay();
    }

    private void Delete(int index, int count)
    {
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
    }
    
    private void InsertText(char character)
    {
        if (caretIndex == currentValue.Length)
            currentValue.Append(character);
        else currentValue.Insert(caretIndex, character);
        caretIndex++;
        UpdateDisplay();
    }
    
    private void UpdateDisplay()
    {
        this.display.Text = currentValue.ToString();
    }
    
    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.RequestFocus();
    }

    public void OnFocusGained(FocusEvent e)
    {
        focused = true;
        InvalidateGeometry();
    }

    public void OnFocusLost(FocusEvent e)
    {
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
        var line = display.GetLineAtIndex(caretIndex);
        caretIndex = display.GetLineStart(line);
        InvalidateLayout();
    }

    private void MoveToLineEnd()
    {
        var line = display.GetLineAtIndex(caretIndex);
        caretIndex = display.GetLineStart(line) + display.GetLineLength(line);
        InvalidateLayout();
    }

    private void MoveUp(int lines)
    {
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

    private void MoveDown(int lines)
    {
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
    
    public void OnKeyDown(KeyEvent e)
    {
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
                if (caretIndex > 0)
                {
                    caretIndex--;
                    InvalidateLayout();
                }
                break;
            }
            case Keys.Right:
            {
                if (caretIndex < currentValue.Length)
                {
                    caretIndex++;
                    InvalidateLayout();
                }
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
        
        if (e.Key == Keys.Enter)
        {
            HandleEnter();
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