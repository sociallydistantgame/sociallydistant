using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace AcidicGUI.VisualStyles;

public interface IVisualStyle : IFontFamilyProvider
{
    int SliderThickness { get; }
    Point ToggleSize { get; }
    Point SwitchSize { get; }
    Font? IconFont { get; }
    Padding DropdownButtonPadding { get; }
    Color SelectionColor { get; }
    Color TextSelectionBackground { get; }
    Color TextSelectionForeground { get; }
    
    int ScrollBarSize { get; }

    Color GetTextColor(Widget? widget = null);

    void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper);

    void DrawScrollBar(
        Widget widget,
        GeometryHelper geometry,
        LayoutRect scrollBarArea,
        int scrollOffset,
        int scrollViewHeight
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

    void DrawSlider(
        Slider widget,
        GeometryHelper geometry,
        bool isHovered,
        bool isPressed,
        bool isVertical,
        float value
    );
}