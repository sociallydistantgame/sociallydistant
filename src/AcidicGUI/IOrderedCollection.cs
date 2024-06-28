namespace AcidicGUI;

public interface IOrderedCollection<T> : ICollection<T>
{
    public T this[int index] { get; }
}