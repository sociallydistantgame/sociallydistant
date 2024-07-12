namespace AcidicGUI.Widgets;

public abstract class DropdownItemView<TItemType> : Widget
{
    public abstract void UpdateView(TItemType data);
}