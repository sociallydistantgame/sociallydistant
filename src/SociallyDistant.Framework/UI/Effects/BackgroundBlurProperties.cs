using AcidicGUI.CustomProperties;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace SociallyDistant.Core.UI.Effects;

public sealed class BackgroundBlurProperties : CustomPropertyObject
{
    private readonly float[] curve      = new float[BackgroundBlurWidgetEffect.MaximumBlurriness * BackgroundBlurWidgetEffect.MaxDistance];
    private          float   blurriness = 4f;

    public float[] ComputedCurve => curve;
    
    public float Blurriness
    {
        get => blurriness;
        set
        {
            blurriness = MathHelper.Clamp(value, 0, BackgroundBlurWidgetEffect.MaximumBlurriness);
            Widget.InvalidateGeometry();

            ComputeCurve();
        }
    }

    private void ComputeCurve()
    {
        const float twoPi = 2 * MathF.PI;
        float deviationSquared = blurriness * blurriness;
        float sampleDivisor = 2 * deviationSquared;
        
        // what do I call this?
        float part1 = 1f / MathF.Sqrt(twoPi * deviationSquared);
        
        int sampleCount = (int)Math.Ceiling(blurriness * BackgroundBlurWidgetEffect.MaxDistance);

        for (var i = 0; i < sampleCount; i++)
        {
            float xSquared = i * i;
            curve[i] = part1 * MathF.Pow(MathF.E, -(xSquared / sampleDivisor));
        }
    }
    
    public BackgroundBlurProperties(Widget owner) : base(owner)
    {
        this.ComputeCurve();
    }
}