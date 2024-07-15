using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Architecture;

namespace SociallyDistant.UI.CharacterCreation;

public sealed class LifepathList : ListAdapter<StackPanel, LifepathViewHolder>
{
    private readonly DataHelper<LifepathAsset> items;

    public event Action<LifepathAsset>? OnSelected;
    
    public LifepathList()
    {
        items = new DataHelper<LifepathAsset>(this);
    }

    public void SetItems(IEnumerable<LifepathAsset> source)
    {
        items.SetItems(source);
    }
    
    protected override LifepathViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new LifepathViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(LifepathViewHolder viewHolder)
    {
        LifepathAsset asset = items[viewHolder.ItemIndex];
        viewHolder.UpdateView(asset);
        
        viewHolder.Callback = OnSelectedItem;
    }

    private void OnSelectedItem(LifepathAsset id)
    {
        OnSelected?.Invoke(id);
    }
}