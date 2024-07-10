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

    public void BeforeRebuildGeometry(Widget widget, GuiRenderer renderer, bool isGeometryDirty)
    {
    }

    public void AfterRebuildGeometry(Widget widget, GuiRenderer renderer)
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