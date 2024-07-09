using AcidicGUI.Rendering;
using AcidicGUI.Widgets;

namespace AcidicGUI.Effects;

public interface IWidgetEffect : IEffect
{
    void UpdateParameters(Widget widget, GuiRenderer renderer);

    void BeforeRebuildGeometry(GeometryHelper geometry);
    void AfterRebuildGeometry(GeometryHelper geometry);
}

