using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Architecture;

namespace SociallyDistant.UI.CharacterCreation;

public sealed class LifepathViewHolder : ViewHolder
{
    private readonly LifepathWidget view = new();

    public Action<LifepathAsset>? Callback
    {
        get => view.Callback;
        set => view.Callback = value;
    }
    
    public LifepathViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(LifepathAsset model)
    {
        view.UpdateView(model);
    }
}