using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ServerMemberItem : Widget
{
    private readonly StackPanel root     = new();
    private readonly Avatar     avatar   = new();
    private readonly StackPanel textArea = new();
    private readonly TextWidget name     = new();
    private readonly TextWidget handle   = new();

    public ServerMemberItem()
    {
        root.Direction = Direction.Horizontal;
        root.Spacing = 3;
        avatar.VerticalAlignment = VerticalAlignment.Middle;
        textArea.VerticalAlignment = VerticalAlignment.Middle;
        name.FontWeight = FontWeight.SemiBold;
        avatar.AvatarSize = 32;
        
        Children.Add(root);
        root.ChildWidgets.Add(avatar);
        root.ChildWidgets.Add(textArea);
        textArea.ChildWidgets.Add(name);
        textArea.ChildWidgets.Add(handle);
    }

    public void UpdateView(ServerMember model)
    {
        avatar.AvatarTexture = model.Avatar;
        name.Text = model.Name;
        handle.Text = model.Handle;
    }
}