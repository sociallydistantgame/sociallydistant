using AcidicGUI.Effects;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI.Effects;

namespace SociallyDistant.Core.UI.Common;

public sealed class Avatar : Widget
{
    private DefaultAvatarColorizer? defaultAvatar;
    private Texture2D?              customAvatar;
    private int                     avatarSize = 24;

    public int AvatarSize
    {
        get => avatarSize;
        set
        {
            avatarSize = value;
            InvalidateLayout();
        }
    }

    public Texture2D? AvatarTexture
    {
        get => customAvatar;
        set
        {
            customAvatar = value;
            InvalidateLayout();
        }
    }
    
    public Avatar()
    {
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        if (defaultAvatar == null)
            defaultAvatar = DefaultAvatarColorizer.GetEffect(Application.Instance.Context);

        this.RenderEffect = customAvatar == null
            ? defaultAvatar
            : null;
        
        return new Vector2(avatarSize, avatarSize);
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        var x = ContentArea.Left + (ContentArea.Width - avatarSize) / 2;
        var y = ContentArea.Top + (ContentArea.Height - avatarSize) / 2;

        geometry.AddRoundedRectangle(new LayoutRect(x,        y, avatarSize, avatarSize), avatarSize * 0.5f, Color.White, customAvatar ?? defaultAvatar.MapTexture);
    }
}