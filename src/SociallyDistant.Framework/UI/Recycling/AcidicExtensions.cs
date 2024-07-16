using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling;

public static class AcidicExtensions
{
    public static void SetWidgets<TContainer>(this RecyclableWidgetList<TContainer> widgetList, IEnumerable<IWidget> widgets)
        where TContainer : ContainerWidget, new()
    {
        widgetList.SetWidgets(widgets.Select(w=>w.Build()));
    }
}