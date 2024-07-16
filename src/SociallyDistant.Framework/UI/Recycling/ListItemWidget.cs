using AcidicGUI.ListAdapters;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class ListItemWidget<T> : IWidget
{
    public IWidget? Image { get; set; }
    public bool Selected { get; set; }
    public string Title { get; set; } = string.Empty;
    public T Data { get; set; } = default!;
    public Action<T>? Callback { get; set; }


    public RecyclableWidgetController Build()
    {
        var controller = new ListItemWidgetController<T>();

        controller.Image = Image?.Build();
        controller.Selected = Selected;
        controller.Title = Title;
        controller.Data = Data;
        controller.Callback = Callback;
        
        return controller;
    }
}