using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace AcidicGUI.VisualStyles;

public interface IVisualStyle : IFontFamilyProvider
{
    Vector2 ToggleSize { get; }
    Vector2 SwitchSize { get; }
    Font? IconFont { get; }
    Padding DropdownButtonPadding { get; }
    
    float ScrollBarSize { get; }

    Color GetTextColor(Widget? widget = null);

    void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper);

    void DrawScrollBar(
        Widget widget,
        GeometryHelper geometry,
        LayoutRect scrollBarArea,
        float scrollOffset,
        float scrollViewHeight
    );

    void DrawToggle(
        Toggle toggle,
        GeometryHelper geometry,
        LayoutRect rect,
        bool isHovered,
        bool isPressed,
        bool isFocused,
        bool isChecked
    );

    void DrawToggleSwitch(
        Toggle toggle,
        GeometryHelper geometry,
        LayoutRect rect,
        bool isHovered,
        bool isPressed,
        bool isFocused,
        bool isChecked
    );

    void DrawDropdownItemsBackground(GeometryHelper geometry, LayoutRect rect);

    void DrawDropdownItemBackground(
        Widget widget,
        GeometryHelper geometry,
        bool hovered,
        bool pressed,
        bool selected
    );
}