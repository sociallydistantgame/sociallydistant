using AcidicGUI.Rendering;
using AcidicGUI.Widgets;

namespace AcidicGUI.Effects;

public interface IWidgetEffect : IEffect
{
    void UpdateParameters(Widget widget, GuiRenderer renderer);

    void BeforeRebuildGeometry(Widget widget, GuiRenderer renderer, bool isGeometryDirty);
    void AfterRebuildGeometry(Widget widget, GuiRenderer renderer);
}

