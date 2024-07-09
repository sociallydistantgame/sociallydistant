using AcidicGUI.CustomProperties;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace SociallyDistant.Core.UI.Effects;

public sealed class BackgroundBlurProperties : CustomPropertyObject
{
    private float blurAmount = 4;

    public float BlurAmount
    {
        get => blurAmount;
        set
        {
            blurAmount = MathHelper.Clamp(value, 0, 4);
            Widget.InvalidateGeometry();
        }
    }

    public BackgroundBlurProperties(Widget owner) : base(owner)
    {
    }
}