using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class AvatarWidgetController : RecyclableWidgetController
{
    private Avatar? avatar;
    
    public AvatarSize Size { get; set; }
    public Texture2D? AvatarTexture { get; set; }
    public Color AvatarColor { get; set; }
    
    public override void Build(ContentWidget destination)
    {
        avatar = GetWidget<Avatar>();

        avatar.AvatarSize = (int)Size;
        avatar.AvatarTexture = AvatarTexture;
        
        destination.Content = avatar;
    }

    public override void Recycle()
    {
        if (avatar != null)
        {
            Recyclewidget(avatar);
        }

        avatar = null;
    }
}