using System.Collections.Immutable;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace AcidicGUI.VisualStyles;

internal sealed class FallbackVisualStyle : IVisualStyle
{
    public int SliderThickness => 12;
    public Point ToggleSize => new Point(18, 18);
    public Point SwitchSize => ToggleSize;
    public Font? IconFont => null;
    public Padding DropdownButtonPadding { get; } = 3;
    public Color SelectionColor => Color.LightBlue;
    public Color TextSelectionBackground => Color.Blue;
    public Color TextSelectionForeground => Color.White;
    public IFontFamily? FallbackFont { get; set; }
    
    public IFontFamily GetFont(PresetFontFamily family)
    {
        if (FallbackFont == null)
            throw new InvalidOperationException("The font for the FallbackVisualStyle has not been set.");

        return FallbackFont;
    }

    public int ScrollBarSize => 18;

    public Color GetTextColor(Widget? widget = null)
    {
        return Color.White;
    }

    public void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper)
    {
        // stub
    }

    public void DrawScrollBar(
        Widget widget,
        GeometryHelper geometry,
        LayoutRect scrollBarArea,
        int scrollOffset,
        int scrollViewHeight
    )
    {
        int barHeight = scrollBarArea.Height / scrollViewHeight * scrollBarArea.Height;
        int barOffset = (scrollOffset / scrollViewHeight) * scrollBarArea.Height;

        geometry.AddQuad(scrollBarArea, Color.Gray);

        geometry.AddQuad(new LayoutRect(scrollBarArea.Left, scrollBarArea.Top + barOffset, scrollBarArea.Width, barHeight), Color.White);
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
        geometry.AddQuad(rect, Color.SlateGray);

        if (isPressed)
        {
            geometry.AddQuad(rect, Color.White * 0.15f);
        }
        else if (isHovered)
        {
            geometry.AddQuad(rect, Color.White * 0.25f);
        }

        geometry.AddQuadOutline(rect, 1, isFocused
            ? Color.White
            : Color.LightGray);

        if (isChecked)
        {
            geometry.AddQuad(new LayoutRect(rect.Left + 3, rect.Top + 3, rect.Width - 6, rect.Height - 6), Color.White);
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
        DrawToggle(toggle, geometry, rect, isHovered, isPressed, isFocused, isChecked);
    }

    public void DrawDropdownItemsBackground(GeometryHelper geometry, LayoutRect rect)
    {
        geometry.AddQuad(rect, Color.DimGray);
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
            geometry.AddQuad(widget.ContentArea, Color.LightGray);

        if (hovered)
            geometry.AddQuad(widget.ContentArea, Color.Gray * 0.25f);
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
        var quarterThickness = SliderThickness / 4;
        
        if (isVertical)
        {
            geometry.AddQuad(new LayoutRect(widget.ContentArea.Left + ((widget.ContentArea.Width - quarterThickness) / 2), widget.ContentArea.Top,                                                            quarterThickness,         widget.ContentArea.Height), Color.Gray);
            geometry.AddQuad(new LayoutRect(widget.ContentArea.Width,                                                      (int)MathHelper.Lerp(widget.ContentArea.Bottom - 1, widget.ContentArea.Top + 1, value), widget.ContentArea.Width, 2),                         Color.White);
        }
        else
        {
            geometry.AddQuad(new LayoutRect(widget.ContentArea.Left,                                                           widget.ContentArea.Top + ((widget.ContentArea.Height - quarterThickness) / 2), widget.ContentArea.Width, quarterThickness),          Color.Gray);
            geometry.AddQuad(new LayoutRect((int)MathHelper.Lerp(widget.ContentArea.Left + 1, widget.ContentArea.Right - 1, value), widget.ContentArea.Top,                                                        2,                        widget.ContentArea.Height), Color.White);
        }
    }
}