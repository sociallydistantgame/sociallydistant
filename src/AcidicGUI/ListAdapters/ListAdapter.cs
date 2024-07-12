using AcidicGUI.Widgets;

namespace AcidicGUI.ListAdapters;

public abstract class ListAdapter<TContainerWidget, TViewHolder> : Widget,
    INotifyDataChanged
    where TContainerWidget : ContainerWidget, new()
    where TViewHolder : ViewHolder
{
    private readonly RecycleBin<Box>   holderRootsBin  = RecycleBin.Get<Box>();
    private readonly List<TViewHolder> holderList      = new();
    private readonly TContainerWidget  containerWidget = new();

    public TContainerWidget Container => containerWidget;
    
    public ListAdapter()
    {
        Children.Add(containerWidget);
    }

    public abstract TViewHolder CreateViewHolder(int itemIndex, Box rootWidget);
    public abstract void UpdateView(TViewHolder viewHolder);

    protected virtual void BeforeRemoveItem(TViewHolder viewHolder)
    { }
    
    public void NotifyCountChanged(int newCount)
    {
        while (holderList.Count > newCount)
        {
            RemoveHolder(holderList[^1]);
            holderList.RemoveAt(holderList.Count - 1);
        }

        for (var i = 0; i < newCount; i++)
        {
            var holder = null as TViewHolder;

            if (i == holderList.Count)
            {
                var root = holderRootsBin.GetWidget();
                holder = CreateViewHolder(i, root);
                holderList.Add(holder);
                containerWidget.ChildWidgets.Add(root);
            }

            UpdateView(holderList[i]);
        }
    }

    public void NotifyItemChanged(int index)
    {
        var holder = holderList[index];
        UpdateView(holder);
    }

    private void RemoveHolder(TViewHolder holder)
    {
        BeforeRemoveItem(holder);
        containerWidget.ChildWidgets.Remove(holder.Root);
        holderRootsBin.Recycle(holder.Root);
    }
}