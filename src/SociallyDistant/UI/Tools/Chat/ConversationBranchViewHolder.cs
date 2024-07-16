using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Chat;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ConversationBranchViewHolder : ViewHolder
{
    private readonly ConversationBranchItem view = new();

    public bool Selected
    {
        get => view.Selected;
        set => view.Selected = value;
    }

    public Action<IBranchDefinition>? Callback
    {
        get => view.Callback;
        set => view.Callback = value;
    }
    
    public ConversationBranchViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
    }

    public void UpdateView(IBranchDefinition branch)
    {
        view.UpdateView(branch);
    }
}