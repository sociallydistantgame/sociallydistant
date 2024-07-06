using System.Net.Mime;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Widgets;

public sealed class Image : Widget
{
    private Texture2D? texture;

    public Texture2D? Texture
    {
        get => texture;
        set
        {
            texture = value;
            InvalidateLayout();
        }
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        if (texture == null)
            return Vector2.Zero;

        return new Vector2(texture.Width, texture.Height);
    }

    protected override void RebuildGeometry(GeometryHelper geometry)
    {
        geometry.AddQuad(ContentArea, Color.Wheat, texture);
    }
}