using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;

namespace AcidicGUI.VisualStyles;

internal sealed class FallbackVisualStyle : IVisualStyle
{
    public Font? FallbackFont { get; set; }
    
    public Font GetFont(FontPreset presetFont)
    {
        if (FallbackFont == null)
            throw new InvalidOperationException("The font for the FallbackVisualStyle has not been set.");

        return FallbackFont;
    }

    public void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper)
    {
        // stub
    }
}