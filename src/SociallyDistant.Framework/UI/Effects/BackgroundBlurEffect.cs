using AcidicGUI.Effects;
using AcidicGUI.Rendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework.Graphics;

namespace SociallyDistant.Core.UI.Effects;

public sealed class BackgroundBlurEffect : MonoGameEffect,
    IWidgetEffect
{
    // TODO
    public BackgroundBlurEffect(Effect underlyingEffect, int techniqueIndex = 0) : base(underlyingEffect, techniqueIndex)
    {
    }

    public void UpdateParameters(Widget widget, GuiRenderer renderer)
    {
        throw new NotImplementedException();
    }

    public void BeforeRebuildGeometry(GeometryHelper geometry)
    {
        throw new NotImplementedException();
    }

    public void AfterRebuildGeometry(GeometryHelper geometry)
    {
        throw new NotImplementedException();
    }
}