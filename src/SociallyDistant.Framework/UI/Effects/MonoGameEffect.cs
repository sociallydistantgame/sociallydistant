using AcidicGUI.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SociallyDistant.Core.UI.Effects;

public class MonoGameEffect : 
    IEffect,
    IDisposable
{
    public static readonly string          TransformMatrixParameterName = "TransformMatrix";
    public static readonly string          OpacityParameterName         = "Opacity";
    private readonly       Effect          underlyingEffect;
    private readonly       int             techniqueIndex;
    private readonly       EffectParameter transformMatrixParameter;
    private readonly       EffectParameter opacityParameter;
    private                Viewport        lastViewport;
    private                Matrix          transformMatrix;
    
    public int PassesCount => underlyingEffect.Techniques[techniqueIndex].Passes.Count;

    public MonoGameEffect(Effect underlyingEffect, int techniqueIndex = 0)
    {
        this.underlyingEffect = underlyingEffect;
        this.techniqueIndex = techniqueIndex;

        transformMatrixParameter = underlyingEffect.Parameters[TransformMatrixParameterName];
        opacityParameter = underlyingEffect.Parameters[OpacityParameterName];

    }
    
    public void Dispose()
    {
        underlyingEffect.Dispose();
    }

    public void UpdateOpacity(float opacity)
    {
        opacityParameter.SetValue(opacity);
    }
    
    public void Use(int pass)
    {
        underlyingEffect.Techniques[techniqueIndex].Passes[pass].Apply();

        Viewport viewport = underlyingEffect.GraphicsDevice.Viewport;

        if (viewport.Width != lastViewport.Width || viewport.Height != lastViewport.Height)
        {
            lastViewport = viewport;

            Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1, out transformMatrix);
        }

        transformMatrixParameter.SetValue(transformMatrix);
    }
}