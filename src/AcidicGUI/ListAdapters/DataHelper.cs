using System.Collections;

namespace AcidicGUI.ListAdapters;

public sealed class DataHelper<T> : IReadOnlyList<T>
{
    private readonly INotifyDataChanged destination;
    private readonly List<T>            dataList = new();
        
    public DataHelper(INotifyDataChanged destination)
    {
        this.destination = destination;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return dataList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => dataList.Count;

    public T this[int index] => dataList[index];

    public void SetItems(IEnumerable<T> newItems)
    {
        this.dataList.Clear();
        this.dataList.AddRange(newItems);

        destination.NotifyCountChanged(dataList.Count);
    }
}