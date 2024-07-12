using System.Collections.Immutable;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace AcidicGUI.VisualStyles;

internal sealed class FallbackVisualStyle : IVisualStyle
{
    public Vector2 ToggleSize => new Vector2(18, 18);
    public Vector2 SwitchSize => ToggleSize;
    public Font? IconFont => null;
    public Padding DropdownButtonPadding { get; } = 3;
    public IFontFamily? FallbackFont { get; set; }
    
    public IFontFamily GetFont(PresetFontFamily family)
    {
        if (FallbackFont == null)
            throw new InvalidOperationException("The font for the FallbackVisualStyle has not been set.");

        return FallbackFont;
    }

    public float ScrollBarSize => 18;

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
        float scrollOffset,
        float scrollViewHeight
    )
    {
        float barHeight = scrollBarArea.Height / scrollViewHeight * scrollBarArea.Height;
        float barOffset = (scrollOffset / scrollViewHeight) * scrollBarArea.Height;

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
}