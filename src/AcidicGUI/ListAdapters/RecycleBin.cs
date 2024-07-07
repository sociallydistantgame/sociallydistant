using AcidicGUI.Widgets;

namespace AcidicGUI.ListAdapters;

public static class RecycleBin
{
    private static readonly Dictionary<Type, object> bins = new();
    
    public static RecycleBin<T> Get<T>()
        where T : Widget, new()
    {
        if (bins.TryGetValue(typeof(T), out object? bin))
        {
            if (bin is not RecycleBin<T> typedBin)
                throw new Exception($"Someone used the wrong recycle bin to recycle something, so we're going to die.");

            return typedBin;
        }

        var newBin = new RecycleBin<T>();
        
        bins.Add(typeof(T), newBin);
        
        return newBin;
    }
}

public sealed class RecycleBin<T>
    where T : Widget, new()
{
    private readonly Queue<T> bin = new();

    internal RecycleBin()
    {
    }

    public void Recycle(T instance)
    {
        if (instance.GuiManager != null)
            throw new InvalidOperationException("You must not recycle a top-level widget.");

        if (instance.Parent != null)
            throw new InvalidOperationException("You must not recycle a widget that's added to a parent.");

        bin.Enqueue(instance);
    }

    public T GetWidget()
    {
        if (bin.TryDequeue(out T? widget))
            return widget;

        widget = new T();
        return widget;
    }
}