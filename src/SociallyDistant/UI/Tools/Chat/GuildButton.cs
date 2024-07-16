using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class GuildButton : Widget
{
    private readonly Button              button  = new();
    private readonly OverlayWidget       overlay = new();
    private readonly Avatar              avatar  = new();
    private readonly CompositeIconWidget icon    = new();
    private          bool                useAvatar;

    public bool UseAvatar
    {
        get => useAvatar;
        set
        {
            useAvatar = value;
            UpdateViewInternal();
        }
    }

    public CompositeIcon Icon
    {
        get => icon.Icon;
        set => icon.Icon = value;
    }

    public Texture2D? Avatar
    {
        get => avatar.AvatarTexture;
        set => avatar.AvatarTexture = value;
    }
    
    public GuildButton()
    {
        avatar.AvatarSize = 36;
        icon.IconSize = 36;
        
        Children.Add(button);
        button.Content = overlay;
        overlay.ChildWidgets.Add(avatar);
        overlay.ChildWidgets.Add(icon);

        UpdateViewInternal();
    }

    private void UpdateViewInternal()
    {
        if (useAvatar)
        {
            icon.Visibility = Visibility.Collapsed;
            avatar.Visibility = Visibility.Visible;
        }
        else
        {
            avatar.Visibility = Visibility.Collapsed;
            icon.Visibility = Visibility.Visible;
        }
    }
}