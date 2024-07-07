using AcidicGUI.ListAdapters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class ImageWidget : IWidget
{
    public Texture2D? Texture { get; set; }
    public Color? Color { get; set; }
	
    public RecyclableWidgetController Build()
    {
        return new ImageWidgetController { Texture = Texture, Color = Color };
    }
}