using AcidicGUI.CustomProperties;
using AcidicGUI.Effects;
using AcidicGUI.Rendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.Core.UI.Effects;

public sealed class DefaultAvatarColorizer : MonoGameEffect,
    IWidgetEffect
{
    private static   DefaultAvatarColorizer? instance = null;
    private readonly EffectParameter         foregroundParameter;
    private readonly EffectParameter         backgroundParameter;
    private readonly Texture2D               texture;
    private          Vector4                 foreground;
    private          Vector4                 background;

    public Texture2D MapTexture => texture;
    
    public void UpdateParameters(Widget widget, GuiRenderer renderer)
    {
        var settings = widget.GetCustomProperties<DefaultAvatarProperties>();

        foreground = settings.Foreground.ToVector4();
        background = settings.UseLightBackground
            ? Color.White.ToVector4()
            : Color.Black.ToVector4();
    }

    public void BeforeRebuildGeometry(GeometryHelper geometry)
    {
    }

    public void AfterRebuildGeometry(GeometryHelper geometry)
    {
    }

    public static DefaultAvatarColorizer GetEffect(IGameContext context)
    {
        if (instance != null)
            return instance;

        var effect = context.GameInstance.Content.Load<Effect>("/Core/Shaders/CyanMagentaMap");
        var texture = context.GameInstance.Content.Load<Texture2D>("/Core/Textures/DefaultAvatarMap");

        instance = new DefaultAvatarColorizer(effect, texture);
        return instance;
    }

    protected override void OnUse(int index)
    {
        backgroundParameter.SetValue(background);
        foregroundParameter.SetValue(foreground);
    }

    private DefaultAvatarColorizer(Effect underlyingEffect, Texture2D texture, int techniqueIndex = 0) : base(underlyingEffect, techniqueIndex)
    {
        this.texture = texture;
        foregroundParameter = underlyingEffect.Parameters["Foreground"];
        backgroundParameter = underlyingEffect.Parameters["Background"];
    }
}

public sealed class DefaultAvatarProperties : CustomPropertyObject
{
    private bool  useLightBackground = false;
    private Color foreground         = Color.Cyan;

    public bool UseLightBackground
    {
        get => useLightBackground;
        set => useLightBackground = value;
    }

    public Color Foreground
    {
        get => foreground;
        set
        {
            foreground = value;
            Widget.InvalidateGeometry();
        }
    } 
    
    public DefaultAvatarProperties(Widget owner) : base(owner)
    {
    }
}

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

        OnUse(pass);
    }

    protected virtual void OnUse(int index)
    {
        
    }
}