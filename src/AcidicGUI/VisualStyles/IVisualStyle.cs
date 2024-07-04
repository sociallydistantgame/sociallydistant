using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;

namespace AcidicGUI.VisualStyles;

public interface IVisualStyle : IFontProvider
{
    float ScrollBarSize { get; }
    
    void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper);

    void DrawScrollBar(Widget widget, GeometryHelper geometry, LayoutRect scrollBarArea, float scrollOffset,
        float scrollViewHeight);
}