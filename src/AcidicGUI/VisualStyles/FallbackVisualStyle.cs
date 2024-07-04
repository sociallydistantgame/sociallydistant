using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

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

    public float ScrollBarSize => 18;

    public void DrawWidgetBackground(Widget widget, GeometryHelper geometryHelper)
    {
        // stub
    }

    public void DrawScrollBar(Widget widget, GeometryHelper geometry, LayoutRect scrollBarArea, float scrollOffset,
        float scrollViewHeight)
    {
        float barHeight = scrollBarArea.Height / scrollViewHeight * scrollBarArea.Height;
        float barOffset = (scrollOffset / scrollViewHeight) * scrollBarArea.Height;

        geometry.AddQuad(scrollBarArea, Color.Gray);
        
        geometry.AddQuad(
            new LayoutRect(
                scrollBarArea.Left,
                scrollBarArea.Top + barOffset,
                scrollBarArea.Width,
                barHeight
            ),
            Color.White
        );
    }
}