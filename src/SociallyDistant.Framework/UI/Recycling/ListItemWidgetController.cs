using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class ListItemWidgetController<T> : RecyclableWidgetController
{
    private TwoLineListItemWithIcon? listItem;
    
    public RecyclableWidgetController? Image { get; set; }
    public bool Selected { get; set; }
    public string Title { get; set; } = string.Empty;
    public T Data { get; set; } = default!;
    public Action<T>? Callback { get; set; }
    
    public override void Build(ContentWidget destination)
    {
        listItem = GetWidget<TwoLineListItemWithIcon>();

        if (Image != null)
        {
            Image.Build(listItem.Icon);
        }
        
        listItem.Selected = Selected;
        listItem.Line1 = Title;
        listItem.Callback = OnClick;

        destination.Content = listItem;
    }

    public override void Recycle()
    {
        if (listItem != null)
        {
            listItem.Icon.Content = null;
            listItem.Callback = null;
            Recyclewidget(listItem);
        }

        if (Image != null)
        {
            Image.Recycle();
            Image = null;
        }
        
        listItem = null;
    }

    private void OnClick()
    {
        Callback?.Invoke(Data);
    }
}