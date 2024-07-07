using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class ImageWidgetController : RecyclableWidgetController
{
    public Texture2D? Texture { get; set; }
    public Color? Color { get; set; }


    public override void Build(ContentWidget destination)
    {
    }

    public override void Recycle()
    {
    }
}