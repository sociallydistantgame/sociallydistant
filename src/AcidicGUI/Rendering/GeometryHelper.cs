using AcidicGUI.Layout;
using FontStashSharp.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public class GeometryHelper
{
    private readonly GuiMeshBuilder whiteMesh = new(null);
    private readonly Dictionary<Texture2D, GuiMeshBuilder> meshes = new();

    public GuiMesh ExportMesh()
    {
        var meshList = new GuiSubMesh[meshes.Count + 1];

        meshList[0] = whiteMesh.ExportSubMesh();

        var i = 1;
        foreach (GuiMeshBuilder mesh in meshes.Values)
        {
            meshList[i] = mesh.ExportSubMesh();
            i++;
        }

        return new GuiMesh(meshList);
    }

    private GuiMeshBuilder GetMeshBuilder(Texture2D? texture)
    {
        if (texture == null)
            return whiteMesh;

        if (!meshes.TryGetValue(texture, out GuiMeshBuilder? builder))
        {
            builder = new GuiMeshBuilder(texture);
            meshes.Add(texture, builder);
        }

        return meshes[texture];
    }
    
    public void AddRoundedRectangle(LayoutRect rectangle, float uniformRadius, Color color, Texture2D? texture = null)
    {
        AddRoundedRectangle(rectangle, uniformRadius, uniformRadius, uniformRadius, uniformRadius, color, texture);
    }

    private void AddQuad(LayoutRect rectangle, Color color, Texture2D? texture = null)
    {
        var mesh = GetMeshBuilder(texture);
        
        int tl = mesh.AddVertex(new Vector2(rectangle.Left, rectangle.Top), color);
        int tr = mesh.AddVertex(new Vector2(rectangle.Right, rectangle.Top), color);
        int bl = mesh.AddVertex(new Vector2(rectangle.Left, rectangle.Bottom), color);
        int br = mesh.AddVertex(new Vector2(rectangle.Right, rectangle.Bottom), color);
        
        mesh.AddQuad(tl, tr, bl, br);
    }
    
    public void AddRoundedRectangle(LayoutRect rectangle, float radiusTopLeft, float radiusTopRight,
        float radiusBottomLeft, float radiusBottomRight, Color color, Texture2D? texture = null)
    {
        var mesh = GetMeshBuilder(texture);
        
        float halfWidth = rectangle.Width / 2;
        float halfHeight = rectangle.Height / 2;

        float smallerHalf = MathF.Min(halfWidth, halfHeight);

        radiusTopLeft = MathHelper.Clamp(radiusTopLeft, 0, smallerHalf);
        radiusBottomLeft = MathHelper.Clamp(radiusBottomLeft, 0, smallerHalf);
        radiusTopRight = MathHelper.Clamp(radiusTopRight, 0, smallerHalf);
        radiusBottomRight = MathHelper.Clamp(radiusBottomRight, 0, smallerHalf);

        if (radiusTopLeft <= 0 && radiusTopRight <= 0 && radiusBottomLeft <= 0 && radiusBottomRight <= 0)
        {
            AddQuad(rectangle, color);
            return;
        }
        
        const int segments = 16;

        int innerTL = mesh.AddVertex(new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top + radiusTopLeft), color);
        int innerTR = mesh.AddVertex(new Vector2(rectangle.Right - radiusTopRight, rectangle.Top + radiusTopRight), color);
        int innerBL = mesh.AddVertex(new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom - radiusBottomLeft), color);
        int innerBR = mesh.AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom - radiusBottomRight), color);

        int outerTL1 = mesh.AddVertex(new Vector2(rectangle.Left, rectangle.Top + radiusTopLeft), color);
        int outerTL2 = mesh.AddVertex(new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top), color);
        int outerTR1 = mesh.AddVertex(new Vector2(rectangle.Right, rectangle.Top + radiusTopRight), color);
        int outerTR2 = mesh.AddVertex(new Vector2(rectangle.Right - radiusTopRight, rectangle.Top), color);
        int outerBL1 = mesh.AddVertex(new Vector2(rectangle.Left, rectangle.Bottom - radiusBottomLeft), color);
        int outerBL2 = mesh.AddVertex(new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom), color);
        int outerBR1 = mesh.AddVertex(new Vector2(rectangle.Right, rectangle.Bottom - radiusBottomRight), color);
        int outerBR2 = mesh.AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom), color);
        
        mesh.AddQuad(outerTL1, innerTL, outerBL1, innerBL);
        mesh.AddQuad(outerTL2, outerTR2, innerTL, innerTR);
        mesh.AddQuad(innerTR, outerTR1, innerBR, outerBR1);
        mesh.AddQuad(innerBL, innerBR, outerBL2, outerBR2);
        mesh.AddQuad(innerTL, innerTR, innerBL, innerBR);
        
        AddQuarterCircleAroundPivotVertices(mesh, innerTL, outerTL2, radiusTopLeft, -1, -1, true, color);
        AddQuarterCircleAroundPivotVertices(mesh, innerTR, outerTR2, radiusTopRight, 1, -1, false, color);
        AddQuarterCircleAroundPivotVertices(mesh, innerBL, outerBL2, radiusBottomLeft, -1, 1, false, color);
        AddQuarterCircleAroundPivotVertices(mesh, innerBR, outerBR2, radiusBottomRight, 1, 1, true, color);
    }

    private void AddQuarterCircleAroundPivotVertices(GuiMeshBuilder mesh, int pivotVertex, int extent, float radius, float directionH, float directionV, bool reverseWinding, Color color)
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
            
            int next = mesh.AddVertex(new Vector2(x1, y1), color);

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
}