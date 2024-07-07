using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace AcidicGUI.VisualStyles;

public interface IVisualStyle : IFontFamilyProvider
{
    Font? IconFont { get; }
    
    float ScrollBarSize { get; }

    Color GetTextColor(Widget? widget = null);
    
    void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper);

    void DrawScrollBar(Widget widget, GeometryHelper geometry, LayoutRect scrollBarArea, float scrollOffset,
        float scrollViewHeight);
}