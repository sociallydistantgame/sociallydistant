using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Chat;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ConversationBranchItem : Widget
{
    private readonly StackPanel         stack   = new();
    private readonly Avatar             avatar  = new();
    private readonly TextWidget         message = new();
    private readonly Emblem             emblem  = new();
    private readonly ListItem           root    = new();
    private          IBranchDefinition? branch;

    public bool Selected
    {
        get => root.IsActive;
        set => root.IsActive = value;
    }
    
    public Action<IBranchDefinition>? Callback { get; set; }

    public ConversationBranchItem()
    {
        avatar.AvatarSize = 16;
        stack.Direction = Direction.Horizontal;
        stack.Spacing = 6;
        avatar.VerticalAlignment = VerticalAlignment.Middle;
        emblem.VerticalAlignment = VerticalAlignment.Middle;
        message.VerticalAlignment = VerticalAlignment.Middle;
        message.WordWrapping = true;
        
        Children.Add(root);
        root.Content = stack;
        stack.ChildWidgets.Add(avatar);
        stack.ChildWidgets.Add(emblem);
        stack.ChildWidgets.Add(message);
        
        root.ClickCallback = OnClick;
    }

    private void OnClick()
    {
        if (branch == null)
            return;
        
        Callback?.Invoke(branch);
    }

    public void UpdateView(IBranchDefinition model)
    {
        this.branch = model;

        avatar.AvatarTexture = model.Target.Picture;
        message.Text = model.Message;
        emblem.Text = model.Target.ChatName.ToUpper();
    }
}