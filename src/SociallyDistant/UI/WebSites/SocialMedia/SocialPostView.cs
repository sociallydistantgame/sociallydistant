using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.UI.Documents;

namespace SociallyDistant.UI.WebSites.SocialMedia;

public sealed class SocialPostView : Widget
{
    private readonly StackPanel                  root     = new();
    private readonly StackPanel                  header   = new();
    private readonly Avatar                      avatar   = new();
    private readonly TextWidget                  username = new();
    private readonly TextWidget                  name     = new();
    private readonly DocumentAdapter<StackPanel> document = new();

    public SocialPostView()
    {
        root.Spacing = 6;
        name.FontWeight = FontWeight.SemiBold;
        username.FontWeight = FontWeight.Light;
        header.Direction = Direction.Horizontal;
        avatar.AvatarSize = 32;
        avatar.VerticalAlignment = VerticalAlignment.Middle;
        name.VerticalAlignment = VerticalAlignment.Middle;
        username.VerticalAlignment = VerticalAlignment.Middle;
        header.Spacing = 3;
        
        Children.Add(root);
        root.ChildWidgets.Add(header);
        header.ChildWidgets.Add(avatar);
        header.ChildWidgets.Add(name);
        header.ChildWidgets.Add(username);
        root.ChildWidgets.Add(document);
    }
    
    public void UpdateView(SocialPostModel model)
    {
        avatar.AvatarTexture = model.Avatar;
        name.Text = model.Name;
        username.Text = model.Handle;
        document.ShowDocument(model.Document);
    }
}