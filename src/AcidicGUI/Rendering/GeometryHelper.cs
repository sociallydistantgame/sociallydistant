using AcidicGUI.Layout;
using FontStashSharp.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public class GeometryHelper : IFontStashRenderer2
{
    private readonly GuiMeshBuilder                        whiteMesh;
    private readonly Dictionary<Texture2D, GuiMeshBuilder> meshes = new();
    private readonly GuiRenderer                           guiRenderer;
    private readonly bool                                  desaturate;
    private readonly LayoutRect?                           clipRect;
    private readonly List<GuiSubMesh>                      flushedMeshes = new();

    public float Layer => guiRenderer.Layer;
    
    public GeometryHelper(GuiRenderer guiRenderer, bool desaturate, LayoutRect? clipRect)
    {
        this.desaturate = desaturate;
        this.guiRenderer = guiRenderer;
        this.clipRect = clipRect;
        
        whiteMesh = new GuiMeshBuilder(this, null, guiRenderer.GetVertexCount(null), desaturate);
    }

    public void PushLayer()
    {
        guiRenderer.PushLayer();
    }

    private void Flush()
    {
        flushedMeshes.Add(whiteMesh.ExportSubMesh());
        whiteMesh.Clear();

        foreach (GuiMeshBuilder mesh in meshes.Values)
        {
            flushedMeshes.Add(mesh.ExportSubMesh());
            mesh.Clear();
        }
    }
    
    public GuiMesh ExportMesh()
    {
        Flush();

        var meshList = flushedMeshes.ToArray();

        return new GuiMesh(meshList, clipRect);
    }

    public GuiMeshBuilder GetMeshBuilder(Texture2D? texture)
    {
        if (texture == null)
            return whiteMesh;

        if (!meshes.TryGetValue(texture, out GuiMeshBuilder? builder))
        {
            builder = new GuiMeshBuilder(this, texture, guiRenderer.GetVertexCount(texture), desaturate);
            meshes.Add(texture, builder);
        }

        return meshes[texture];
    }
    
    public void AddRoundedRectangle(LayoutRect rectangle, float uniformRadius, Color color, Texture2D? texture = null)
    {
        AddRoundedRectangle(rectangle, uniformRadius, uniformRadius, uniformRadius, uniformRadius, color, texture);
    }

    public void AddQuadOutline(
        LayoutRect rectangle,
        float thickness,
        Color color,
        Texture2D? texture = null
    )
    {
        float smallerHalf = Math.Min(rectangle.Width, rectangle.Height) / 2;
        if (smallerHalf <= thickness)
        {
            AddQuad(rectangle, color, texture);
            return;
        }

        float texelWidth = 1f / (texture?.Width ?? 1f);
        float texelHeight = 1f / (texture?.Height ?? 1f);

        var mesh = GetMeshBuilder(texture);

        var tl = mesh.AddVertex(new Vector2(rectangle.Left,                   rectangle.Top),                color, new Vector2(0,                          0));
        var tr = mesh.AddVertex(new Vector2(rectangle.Right,                  rectangle.Top),                color, new Vector2(1,                          0));
        var bl = mesh.AddVertex(new Vector2(rectangle.Left,                   rectangle.Bottom),             color, new Vector2(0,                          1));
        var br = mesh.AddVertex(new Vector2(rectangle.Right,                  rectangle.Bottom),             color, new Vector2(1,                          1));
        var tlInner = mesh.AddVertex(new Vector2(rectangle.Left + thickness,  rectangle.Top + thickness),    color, new Vector2(texelWidth * thickness,     texelHeight * thickness));
        var trInner = mesh.AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Top + thickness),    color, new Vector2(1 - texelWidth * thickness, texelHeight * thickness));
        var blInner = mesh.AddVertex(new Vector2(rectangle.Left + thickness,  rectangle.Bottom - thickness), color, new Vector2(texelWidth * thickness,     1 - texelHeight * thickness));
        var brInner = mesh.AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Bottom - thickness), color, new Vector2(1 - texelWidth * thickness, 1 - texelHeight * thickness));

        mesh.AddQuad(tl,      tr,      tlInner, trInner);
        mesh.AddQuad(tl,      tlInner, bl,      blInner);
        mesh.AddQuad(trInner, tr,      brInner, br);
        mesh.AddQuad(blInner, brInner, bl,      br);
    }

    public void AddQuad(LayoutRect rectangle, Color color, Texture2D? texture = null)
    {
        var mesh = GetMeshBuilder(texture);

        int tl = mesh.AddVertex(new Vector2(rectangle.Left,  rectangle.Top),    color, new Vector2(0, 0));
        int tr = mesh.AddVertex(new Vector2(rectangle.Right, rectangle.Top),    color, new Vector2(1, 0));
        int bl = mesh.AddVertex(new Vector2(rectangle.Left,  rectangle.Bottom), color, new Vector2(0, 1));
        int br = mesh.AddVertex(new Vector2(rectangle.Right, rectangle.Bottom), color, new Vector2(1, 1));

        mesh.AddQuad(tl, tr, bl, br);
    }

    public void AddRoundedRectangle(
        LayoutRect rectangle,
        float radiusTopLeft,
        float radiusTopRight,
        float radiusBottomLeft,
        float radiusBottomRight,
        Color color,
        Texture2D? texture = null
    )
    {
        var mesh = GetMeshBuilder(texture);

        float halfWidth = rectangle.Width / 2;
        float halfHeight = rectangle.Height / 2;

        float smallerHalf = MathF.Min(halfWidth, halfHeight);

        radiusTopLeft = MathHelper.Clamp(radiusTopLeft,         0, smallerHalf);
        radiusBottomLeft = MathHelper.Clamp(radiusBottomLeft,   0, smallerHalf);
        radiusTopRight = MathHelper.Clamp(radiusTopRight,       0, smallerHalf);
        radiusBottomRight = MathHelper.Clamp(radiusBottomRight, 0, smallerHalf);

        if (radiusTopLeft <= 0 && radiusTopRight <= 0 && radiusBottomLeft <= 0 && radiusBottomRight <= 0)
        {
            AddQuad(rectangle, color);
            return;
        }

        float radiusTopLeftUV = radiusTopLeft / rectangle.Width;
        float radiusTopRightUV = radiusTopRight / rectangle.Width;
        float radiusBottomLeftUV = radiusBottomLeft / rectangle.Width;
        float radiusBottomRightUV = radiusBottomRight / rectangle.Width;
        
        
        float texelWidth = 1f /  (float)(texture?.Width ?? 1f);
        float texelHeight = 1f / (float) (texture?.Height ?? 1f);
        
        int innerTL = mesh.AddVertex(new Vector2(rectangle.Left + radiusTopLeft,      rectangle.Top + radiusTopLeft),        color, new Vector2(radiusTopLeftUV,         radiusTopLeftUV));
        int innerTR = mesh.AddVertex(new Vector2(rectangle.Right - radiusTopRight,    rectangle.Top + radiusTopRight),       color, new Vector2(1 - radiusTopRightUV,    radiusTopRightUV));
        int innerBL = mesh.AddVertex(new Vector2(rectangle.Left + radiusBottomLeft,   rectangle.Bottom - radiusBottomLeft),  color, new Vector2(radiusBottomLeftUV,      1 - radiusBottomLeftUV));
        int innerBR = mesh.AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom - radiusBottomRight), color, new Vector2(1 - radiusBottomRightUV, 1 - radiusBottomRightUV));

        int outerTL1 = mesh.AddVertex(new Vector2(rectangle.Left,                      rectangle.Top + radiusTopLeft),        color, new Vector2(0,                       radiusTopLeftUV));
        int outerTL2 = mesh.AddVertex(new Vector2(rectangle.Left + radiusTopLeft,      rectangle.Top),                        color, new Vector2(radiusTopLeftUV,         0));
        int outerTR1 = mesh.AddVertex(new Vector2(rectangle.Right,                     rectangle.Top + radiusTopRight),       color, new Vector2(1,                       radiusTopRightUV));
        int outerTR2 = mesh.AddVertex(new Vector2(rectangle.Right - radiusTopRight,    rectangle.Top),                        color, new Vector2(1 - radiusTopRightUV,    0));
        int outerBL1 = mesh.AddVertex(new Vector2(rectangle.Left,                      rectangle.Bottom - radiusBottomLeft),  color, new Vector2(0,                       1 - radiusBottomLeftUV));
        int outerBL2 = mesh.AddVertex(new Vector2(rectangle.Left + radiusBottomLeft,   rectangle.Bottom),                     color, new Vector2(radiusBottomLeftUV,      1));
        int outerBR1 = mesh.AddVertex(new Vector2(rectangle.Right,                     rectangle.Bottom - radiusBottomRight), color, new Vector2(1,                       1 - radiusBottomRightUV));
        int outerBR2 = mesh.AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom),                     color, new Vector2(1 - radiusBottomRightUV, 1));

        mesh.AddQuad(outerTL1, innerTL,  outerBL1, innerBL);
        mesh.AddQuad(outerTL2, outerTR2, innerTL,  innerTR);
        mesh.AddQuad(innerTR,  outerTR1, innerBR,  outerBR1);
        mesh.AddQuad(innerBL,  innerBR,  outerBL2, outerBR2);
        mesh.AddQuad(innerTL,  innerTR,  innerBL,  innerBR);

        AddQuarterCircleAroundPivotVertices(mesh, innerTL, outerTL2, radiusTopLeft,     -1, -1, true,  color, radiusTopLeftUV,         radiusTopLeftUV,         radiusTopLeftUV);
        AddQuarterCircleAroundPivotVertices(mesh, innerTR, outerTR2, radiusTopRight,    1,  -1, false, color, 1 - radiusTopRightUV,    radiusTopRightUV,        radiusTopRightUV);
        AddQuarterCircleAroundPivotVertices(mesh, innerBL, outerBL2, radiusBottomLeft,  -1, 1,  false, color, radiusBottomLeftUV,      1 - radiusBottomLeftUV,  radiusBottomLeftUV);
        AddQuarterCircleAroundPivotVertices(mesh, innerBR, outerBR2, radiusBottomRight, 1,  1,  true,  color, 1 - radiusBottomRightUV, 1 - radiusBottomRightUV, radiusBottomRightUV);
    }

    private void AddQuarterCircleAroundPivotVertices(GuiMeshBuilder mesh, int pivotVertex, int extent, float radius, float directionH, float directionV, bool reverseWinding, Color color, float uvX, float uvY, float texelRadius)
    {
        Vector3 center = mesh[pivotVertex].Position;

        int last = extent;

        const int segments = 16;

        for (var i = 0; i < segments; i++)
        {
            var t = (i / ((float) segments - 1f)) * MathF.PI * 0.5f;
            var x = MathF.Sin(t);
            var y = MathF.Cos(t);
            
            float x1 = x * (radius * directionH) + center.X;
            float y1 = y * (radius * directionV) + center.Y;

            float u = (x * (texelRadius * directionH)) + uvX;
            float v = (y * (texelRadius * directionV)) + uvY;



            int next = mesh.AddVertex(new Vector2(x1, y1), color, new Vector2(u, v));

            if (reverseWinding)
                mesh.AddTriangle(pivotVertex, next, last);
            else
                mesh.AddTriangle(last, next, pivotVertex);

            last = next;
        }
    }
    
    public void AddRoundedRectangleOutline(LayoutRect rectangle, float thickness, float uniformRadius, Color color, Texture2D? texture = null)
    {
        AddRoundedRectangleOutline(rectangle, thickness, uniformRadius, uniformRadius, uniformRadius, uniformRadius,
            color, texture);
    }
    
    public void AddRoundedRectangleOutline(LayoutRect rectangle, float thickness, float radiusTopLeft,
        float radiusTopRight, float radiusBottomLeft, float radiusBottomRight, Color color, Texture2D? texture = null)
    {
        if (thickness <= 0)
            return;
        
        float halfWidth = rectangle.Width / 2;
        float halfHeight = rectangle.Height / 2;

        float smallerHalf = MathF.Min(halfWidth, halfHeight);
        
        if (thickness >= smallerHalf)
        {
            AddRoundedRectangle(rectangle, radiusTopLeft, radiusTopRight, radiusBottomLeft, radiusBottomRight, color, texture);
            return;
        }
        
        radiusTopLeft = MathHelper.Clamp(radiusTopLeft, 0, smallerHalf);
        radiusBottomLeft = MathHelper.Clamp(radiusBottomLeft, 0, smallerHalf);
        radiusTopRight = MathHelper.Clamp(radiusTopRight, 0, smallerHalf);
        radiusBottomRight = MathHelper.Clamp(radiusBottomRight, 0, smallerHalf);

        
        if (radiusTopLeft <= 0 && radiusTopRight <= 0 && radiusBottomLeft <= 0 && radiusBottomRight <= 0)
            return;

        var mesh = GetMeshBuilder(texture);
        
        float offsetTL = MathF.Max(radiusTopLeft, thickness);
        float offsetTR = MathF.Max(radiusTopRight, thickness);
        float offsetBL = MathF.Max(radiusBottomLeft, thickness);
        float offsetBR = MathF.Max(radiusBottomRight, thickness);
        
        int innerTL = mesh.AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Top + thickness), color);
        int innerTR = mesh.AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Top + thickness), color);
        int innerBL = mesh.AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Bottom - thickness), color);
        int innerBR = mesh.AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Bottom - thickness), color);
        int innerTL2 = mesh.AddVertex(new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top + thickness), color);
        int innerTR2 = mesh.AddVertex(new Vector2(rectangle.Right - radiusTopRight, rectangle.Top + thickness), color);
        int innerBL2 = mesh.AddVertex(new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom - thickness), color);
        int innerBR2 = mesh.AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom - thickness), color);
        int innerTL3 = mesh.AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Top + radiusTopLeft), color);
        int innerTR3 = mesh.AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Top + radiusTopRight), color);
        int innerBL3 = mesh.AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Bottom - radiusBottomLeft), color);
        int innerBR3 = mesh.AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Bottom - radiusBottomRight), color);

        int outerTL1 = mesh.AddVertex(new Vector2(rectangle.Left, rectangle.Top + radiusTopLeft), color);
        int outerTL2 = mesh.AddVertex(new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top), color);
        int outerTR1 = mesh.AddVertex(new Vector2(rectangle.Right, rectangle.Top + radiusTopRight), color);
        int outerTR2 = mesh.AddVertex(new Vector2(rectangle.Right - radiusTopRight, rectangle.Top), color);
        int outerBL1 = mesh.AddVertex(new Vector2(rectangle.Left, rectangle.Bottom - radiusBottomLeft), color);
        int outerBL2 = mesh.AddVertex(new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom), color);
        int outerBR1 = mesh.AddVertex(new Vector2(rectangle.Right, rectangle.Bottom - radiusBottomRight), color);
        int outerBR2 = mesh.AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom), color);

        Vector2 cTL = new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top + radiusTopLeft);
        Vector2 cTR = new Vector2(rectangle.Right - radiusTopRight, rectangle.Top + radiusTopRight);
        Vector2 cBL = new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom - radiusBottomLeft);
        Vector2 cBR = new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom - radiusBottomRight);
        
        mesh.AddQuad(outerTL1, innerTL3, outerBL1, innerBL3);
        mesh.AddQuad(outerTL2, outerTR2, innerTL2, innerTR2);
        mesh.AddQuad(innerTR3, outerTR1, innerBR3, outerBR1);
        mesh.AddQuad(innerBL2, innerBR2, outerBL2, outerBR2);
        
        AddRoundedRectangleOutlineCurve(mesh, cTL, innerTL, thickness, radiusTopLeft, -1, -1, true, color);
        AddRoundedRectangleOutlineCurve(mesh, cTR, innerTR, thickness, radiusTopRight, 1, -1, false, color);
        AddRoundedRectangleOutlineCurve(mesh, cBL, innerBL, thickness, radiusBottomLeft, -1, 1, false, color);
        AddRoundedRectangleOutlineCurve(mesh, cBR, innerBR, thickness, radiusBottomRight, 1, 1, true, color);
    }

    private void AddRoundedRectangleOutlineCurve(GuiMeshBuilder mesh, Vector2 center, int innerCorner, float thickness, float radius, float directionH, float directionV, bool reverseWinding, Color color)
    {
        const int segments = 16;

        var connectToInnerCorner = false;
        if (radius < thickness)
        {
            connectToInnerCorner = true;
            thickness = radius;
        }
        
        int currentInner = 0;
        int currentOuter = 0;
        
        for (var i = 0; i < segments; i++)
        {
            var t = (i / ((float) segments - 1f)) * MathF.PI * 0.5f;
            var x = MathF.Sin(t);
            var y = MathF.Cos(t);
            
            var xOuter = x * (radius * directionH) + center.X;
            var yOuter = y * (radius * directionV) + center.Y;
            var xInner = x * ((radius * directionH) + (thickness * -directionH)) + center.X;
            var yInner = y * ((radius * directionV) + (thickness * -directionV)) + center.Y;
            
            int nextInner = mesh.AddVertex(new Vector2(xInner, yInner), color);
            int nextOuter = mesh.AddVertex(new Vector2(xOuter, yOuter), color);

            if (reverseWinding)
            {
                if (i > 0)
                {
                    mesh.AddTriangle(nextInner, currentOuter, currentInner);
                    mesh.AddTriangle(nextOuter, currentOuter, nextInner);
                }

                if (connectToInnerCorner)
                {
                    if (i == 0 || i == segments - 1)
                    {
                        mesh.AddTriangle(innerCorner, nextInner, nextOuter);
                    }
                }
            }
            else
            {
                if (i > 0)
                {
                    mesh.AddTriangle(currentInner, currentOuter, nextInner);
                    mesh.AddTriangle(nextInner, currentOuter, nextOuter);
                }

                if (connectToInnerCorner)
                {
                    if (i == 0 || i == segments - 1)
                    {
                        mesh.AddTriangle(nextOuter, nextInner, innerCorner);
                    }
                }
            }

            currentInner = nextInner;
            currentOuter = nextOuter;
        }
    }

    public void DrawQuad(Texture2D texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight,
        ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight)
    {
        var mesh = GetMeshBuilder(texture);

        int i1 = mesh.AddVertex(topLeft);
        int i2 = mesh.AddVertex(topRight);
        int i3 = mesh.AddVertex(bottomLeft);
        int i4 = mesh.AddVertex(bottomRight);
        
        mesh.AddTriangle(i1, i2, i3);
        mesh.AddTriangle(i3, i2, i4);
    }

    public GraphicsDevice GraphicsDevice => guiRenderer.GraphicsDevice;
}