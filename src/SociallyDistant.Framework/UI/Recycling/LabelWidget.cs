using AcidicGUI.Layout;
using AcidicGUI.ListAdapters;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class TwoLineListItemWithIcon : Widget
{
    private readonly ListItem   root  = new();
    private readonly StackPanel stack = new();
    private readonly Box        icon  = new();
    private readonly StackPanel lines = new();
    private readonly TextWidget line1 = new();
    private readonly TextWidget line2 = new();

    public string Line1
    {
        get => line1.Text;
        set => line1.Text = value;
    }

    public string Line2
    {
        get => line2.Text;
        set => line2.Text = value;
    }

    public Box Icon => icon;

    public bool Selected
    {
        get => root.IsActive;
        set => root.IsActive = value;
    }
    
    public Action? Callback
    {
        get => root.ClickCallback;
        set => root.ClickCallback = value;
    }

    public TwoLineListItemWithIcon()
    {
        line1.WordWrapping = true;
        line2.WordWrapping = true;
        line1.UseMarkup = true;
        line2.UseMarkup = true;
        stack.Direction = Direction.Horizontal;
        stack.Spacing = 6;
        icon.VerticalAlignment = VerticalAlignment.Middle;
        lines.VerticalAlignment = VerticalAlignment.Middle;
        
        Children.Add(root);
        root.Content = stack;
        stack.ChildWidgets.Add(icon);
        stack.ChildWidgets.Add(lines);
        lines.ChildWidgets.Add(line1);
        lines.ChildWidgets.Add(line2);
    }
}

public sealed class LabelWidget : IWidget
{
    public string Text { get; set; } = string.Empty;
    
    public RecyclableWidgetController Build()
    {
        return new LabelWidgetController { LabelText = Text };
    }
}