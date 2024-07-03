using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;

namespace AcidicGUI.VisualStyles;

public interface IVisualStyle : IFontProvider
{
    void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper);
}