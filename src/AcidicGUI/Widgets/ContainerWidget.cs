namespace AcidicGUI.Widgets;

public abstract class ContainerWidget : Widget
{
    public IOrderedCollection<Widget> ChildWidgets => Children;
}