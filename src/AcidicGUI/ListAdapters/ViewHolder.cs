using System.ComponentModel;
using AcidicGUI.Widgets;

namespace AcidicGUI.ListAdapters;

public abstract class ViewHolder
{
    private readonly Box root;
    private readonly int itemIndex;

    public Box Root => root;
    public int ItemIndex => itemIndex;
    
    public ViewHolder(int itemIndex, Box root)
    {
        this.itemIndex = itemIndex;
        this.root = root;
    }
}

public sealed class RecyclableWidgetList<TContainerWidget> : ListAdapter<TContainerWidget, RecyclableWidgetList<TContainerWidget>.RecyclableViewHolder>
    where TContainerWidget : ContainerWidget, new()
{
    private readonly DataHelper<RecyclableWidgetController> controllers;
    
    public RecyclableWidgetList()
    {
        controllers = new DataHelper<RecyclableWidgetController>(this);
    }

    public void SetWidgets(IEnumerable<RecyclableWidgetController> widgets)
    {
        this.controllers.SetItems(widgets);
    }

    protected override RecyclableViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new RecyclableViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(RecyclableViewHolder viewHolder)
    {
        var controller = controllers[viewHolder.ItemIndex];

        if (controller != viewHolder.Controller)
        {
            if (viewHolder.Controller != null)
            {
                viewHolder.Content.Content = null;
                viewHolder.Controller.Recycle();
                viewHolder.Controller = null;
            }
        }

        viewHolder.Controller = controller;
        viewHolder.Controller.Build(viewHolder.Content);
    }
    
    public class RecyclableViewHolder : ViewHolder
    {
        public Box Content { get; private set; }
        public RecyclableWidgetController? Controller { get; set; }
        
        public RecyclableViewHolder(int itemIndex, Box root) : base(itemIndex, root)
        {
            Content = new Box();
            root.Content = Content;
        }
    }
}