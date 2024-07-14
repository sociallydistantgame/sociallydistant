using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.InfoPanel;

namespace SociallyDistant.UI.InfoWidgets;

public sealed class InfoWidgetAdapter : ListAdapter<ScrollView, InfoWidgetViewHolder>
{
    private readonly DataHelper<InfoWidgetData> items;

    public event Action<int>? ItemClosed;
    
    public InfoWidgetAdapter()
    {
        items = new DataHelper<InfoWidgetData>(this);
    }

    public void SetWidgets(IEnumerable<InfoWidgetData> source)
    {
        items.SetItems(source);
    }
    
    protected override InfoWidgetViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new InfoWidgetViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(InfoWidgetViewHolder viewHolder)
    {
        viewHolder.ItemClosed = OnItemClosed;
        viewHolder.UpdateView(items[viewHolder.ItemIndex]);
    }

    private void OnItemClosed(int itemId)
    {
        ItemClosed?.Invoke(itemId);
    }
}