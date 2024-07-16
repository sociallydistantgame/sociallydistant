using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Chat;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ConversationBranchAdapter : ListAdapter<ScrollView, ConversationBranchViewHolder>
{
    private readonly DataHelper<IBranchDefinition> branches;

    public int SelectedItem { get; set; }

    public event Action<IBranchDefinition>? ItemSelected;
    
    public ConversationBranchAdapter()
    {
        branches = new DataHelper<IBranchDefinition>(this);
    }

    public void SetItems(IEnumerable<IBranchDefinition> source)
    {
        branches.SetItems(source);
    }

    protected override ConversationBranchViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new ConversationBranchViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(ConversationBranchViewHolder viewHolder)
    {
        viewHolder.Selected = viewHolder.ItemIndex == SelectedItem;
        viewHolder.UpdateView(branches[viewHolder.ItemIndex]);
        viewHolder.Callback = OnItemSelect;
    }

    private void OnItemSelect(IBranchDefinition branch)
    {
        ItemSelected?.Invoke(branch);
    }
}