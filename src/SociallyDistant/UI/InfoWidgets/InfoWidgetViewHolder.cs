using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.InfoPanel;

namespace SociallyDistant.UI.InfoWidgets;

public sealed class InfoWidgetViewHolder : ViewHolder
{
    private readonly InfoWidgetView view = new();

    public Action<int>? ItemClosed
    {
        get => view.ItemClosed;
        set => view.ItemClosed = value;
    }
    
    public InfoWidgetViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(InfoWidgetData data)
    {
        view.UpdateView(data);
    }
}