namespace AcidicGUI.ListAdapters;

public interface INotifyDataChanged
{
    void NotifyCountChanged(int newCount);
    void NotifyItemChanged(int index);
}