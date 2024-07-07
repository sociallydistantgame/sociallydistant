using System.Collections;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.UI;

public sealed class DockGroup : IList<DockGroup.IconDefinition>
{
    private readonly List<DockGroup.IconDefinition> definitions = new();
    private readonly DockModel model;

    public DockGroup(DockModel model)
    {
        this.model = model;
    }

    /// <inheritdoc />
    public IEnumerator<IconDefinition> GetEnumerator()
    {
        return definitions.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(IconDefinition item)
    {
        definitions.Add(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        definitions.Clear();
    }

    /// <inheritdoc />
    public bool Contains(IconDefinition item)
    {
        return definitions.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(IconDefinition[] array, int arrayIndex)
    {
        definitions.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public bool Remove(IconDefinition item)
    {
        if (!definitions.Remove(item))
            return false;

        return true;
    }

    /// <inheritdoc />
    public int Count => definitions.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public int IndexOf(IconDefinition item)
    {
        return definitions.IndexOf(item);
    }

    /// <inheritdoc />
    public void Insert(int index, IconDefinition item)
    {
        definitions.Insert(index, item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        definitions.RemoveAt(index);
    }

    /// <inheritdoc />
    public IconDefinition this[int index]
    {
        get => definitions[index];
        set => definitions[index] = value;
    }

    public struct IconDefinition
    {
        public CompositeIcon Icon;
        public string Label;
        public bool IsActive;
        public Action? ClickHandler;
        public INotificationGroup? NotificationGroup;
    }
}