using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.WebSites.SocialMedia;

public sealed class ProfilePage : Widget
{
    private readonly FlexPanel  root         = new();
    private readonly StackPanel header       = new();
    private readonly StackPanel pictures     = new();
    private readonly Box        cover        = new();
    private readonly Avatar     avatar       = new();
    private readonly TextWidget name         = new();
    private readonly TextWidget handle       = new();
    private readonly TextWidget bio          = new();
    private readonly Timeline   userTimeline = new();

    public ProfilePage()
    {
        avatar.HorizontalAlignment = HorizontalAlignment.Left;
        header.Margin = new Padding(12, 0, 12, 12);
        cover.SetCustomProperty(CommonColor.Blue);
        cover.SetCustomProperty(WidgetBackgrounds.Common);
        cover.MinimumSize = new Point(0, 200);
        pictures.Spacing = -48;
        avatar.AvatarSize = 96;
        cover.Padding = new Padding(-12,  0, -12, 0);
        pictures.Padding = new Padding(0, 0, 0,   12);

        name.FontWeight = FontWeight.SemiBold;
        
        Children.Add(root);
        root.ChildWidgets.Add(header);
        header.ChildWidgets.Add(pictures);
        pictures.ChildWidgets.Add(cover);
        pictures.ChildWidgets.Add(avatar);
        header.ChildWidgets.Add(name);
        header.ChildWidgets.Add(handle);
        header.ChildWidgets.Add(bio);
        root.ChildWidgets.Add(userTimeline);
    }
    
    public void ShowProfile(IProfile profile)
    {
        name.Text = profile.ChatName;
        handle.Text = profile.ChatUsername;
        avatar.AvatarTexture = profile.Picture;
        bio.Text = profile.Bio;
    }
}