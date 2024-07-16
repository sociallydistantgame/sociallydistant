using AcidicGUI.ListAdapters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Social;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class AvatarWidget : IWidget
{
    public AvatarSize Size { get; set; }
    public Texture2D? AvatarTexture { get; set; }
    public Color AvatarColor { get; set; }
    public RecyclableWidgetController Build()
    {
        var controller = new AvatarWidgetController();

        controller.Size = Size;
        controller.AvatarTexture = AvatarTexture;
        controller.AvatarColor = AvatarColor;
        
        return controller;
    }
}