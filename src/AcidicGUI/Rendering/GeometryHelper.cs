using AcidicGUI.Layout;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public class GeometryHelper
{
    private readonly List<VertexPositionColorTexture> vertices = new();
    private readonly List<int> indices = new();

    public int AddVertex(VertexPositionColorTexture vertex)
    {
        int index = vertices.Count;
        vertices.Add(vertex);
        return index;
    }

    public int AddVertex(Vector3 position, Color color)
    {
        return AddVertex(new VertexPositionColorTexture(position, color, Vector2.Zero));
    }

    public int AddVertex(Vector2 position, Color color)
    {
        return AddVertex(new Vector3(position.X, position.Y, 0), color);
    }

    public void AddTriangle(int a, int b, int c)
    {
        indices.Add(a);
        indices.Add(b);
        indices.Add(c);
    }

    public void AddQuad(int a, int b, int c, int d)
    {
        AddTriangle(a, b, c);
        AddTriangle(b, c, d);
    }

    public GuiMesh ExportMesh()
    {
        return new GuiMesh(vertices.ToArray(), indices.ToArray());
    }

    public void AddRoundedRectangle(LayoutRect rectangle, float uniformRadius, Color color)
    {
        AddRoundedRectangle(rectangle, uniformRadius, uniformRadius, uniformRadius, uniformRadius, color);
    }

    private void AddQuad(LayoutRect rectangle, Color color)
    {
        int tl = AddVertex(new Vector2(rectangle.Left, rectangle.Top), color);
        int tr = AddVertex(new Vector2(rectangle.Right, rectangle.Top), color);
        int bl = AddVertex(new Vector2(rectangle.Left, rectangle.Bottom), color);
        int br = AddVertex(new Vector2(rectangle.Right, rectangle.Bottom), color);
        
        AddQuad(tl, tr, bl, br);
    }
    
    public void AddRoundedRectangle(LayoutRect rectangle, float radiusTopLeft, float radiusTopRight,
        float radiusBottomLeft, float radiusBottomRight, Color color)
    {
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

        int innerTL = AddVertex(new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top + radiusTopLeft), color);
        int innerTR = AddVertex(new Vector2(rectangle.Right - radiusTopRight, rectangle.Top + radiusTopRight), color);
        int innerBL = AddVertex(new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom - radiusBottomLeft), color);
        int innerBR = AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom - radiusBottomRight), color);

        int outerTL1 = AddVertex(new Vector2(rectangle.Left, rectangle.Top + radiusTopLeft), color);
        int outerTL2 = AddVertex(new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top), color);
        int outerTR1 = AddVertex(new Vector2(rectangle.Right, rectangle.Top + radiusTopRight), color);
        int outerTR2 = AddVertex(new Vector2(rectangle.Right - radiusTopRight, rectangle.Top), color);
        int outerBL1 = AddVertex(new Vector2(rectangle.Left, rectangle.Bottom - radiusBottomLeft), color);
        int outerBL2 = AddVertex(new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom), color);
        int outerBR1 = AddVertex(new Vector2(rectangle.Right, rectangle.Bottom - radiusBottomRight), color);
        int outerBR2 = AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom), color);
        
        AddQuad(outerTL1, innerTL, outerBL1, innerBL);
        AddQuad(outerTL2, outerTR2, innerTL, innerTR);
        AddQuad(innerTR, outerTR1, innerBR, outerBR1);
        AddQuad(innerBL, innerBR, outerBL2, outerBR2);
        AddQuad(innerTL, innerTR, innerBL, innerBR);
        
        AddQuarterCircleAroundPivotVertices(innerTL, outerTL2, radiusTopLeft, -1, -1, color);
        AddQuarterCircleAroundPivotVertices(innerTR, outerTR2, radiusTopRight, 1, -1, color);
        AddQuarterCircleAroundPivotVertices(innerBL, outerBL2, radiusBottomLeft, -1, 1, color);
        AddQuarterCircleAroundPivotVertices(innerBR, outerBR2, radiusBottomRight, 1, 1, color);
    }

    private void AddQuarterCircleAroundPivotVertices(int pivotVertex, int extent, float radius, float directionH, float directionV, Color color)
    {
        Vector3 center = vertices[pivotVertex].Position;

        int last = extent;

        const int segments = 16;

        for (var i = 0; i < segments; i++)
        {
            var t = (i / ((float) segments - 1f)) * MathF.PI * 0.5f;
            var x = MathF.Sin(t);
            var y = MathF.Cos(t);
            
            float x1 = x * (radius * directionH) + center.X;
            float y1 = y * (radius * directionV) + center.Y;
            
            int next = AddVertex(new Vector2(x1, y1), color);

            AddTriangle(last, next, pivotVertex);

            last = next;
        }
    }
    
    public void AddRoundedRectangleOutline(LayoutRect rectangle, float thickness, float uniformRadius, Color color)
    {
        AddRoundedRectangleOutline(rectangle, thickness, uniformRadius, uniformRadius, uniformRadius, uniformRadius,
            color);
    }
    
    public void AddRoundedRectangleOutline(LayoutRect rectangle, float thickness, float radiusTopLeft,
        float radiusTopRight, float radiusBottomLeft, float radiusBottomRight, Color color)
    {
        if (thickness <= 0)
            return;
        
        float halfWidth = rectangle.Width / 2;
        float halfHeight = rectangle.Height / 2;

        float smallerHalf = MathF.Min(halfWidth, halfHeight);
        
        if (thickness >= smallerHalf)
        {
            AddRoundedRectangle(rectangle, radiusTopLeft, radiusTopRight, radiusBottomLeft, radiusBottomRight, color);
            return;
        }
        
        radiusTopLeft = MathHelper.Clamp(radiusTopLeft, 0, smallerHalf);
        radiusBottomLeft = MathHelper.Clamp(radiusBottomLeft, 0, smallerHalf);
        radiusTopRight = MathHelper.Clamp(radiusTopRight, 0, smallerHalf);
        radiusBottomRight = MathHelper.Clamp(radiusBottomRight, 0, smallerHalf);

        
        if (radiusTopLeft <= 0 && radiusTopRight <= 0 && radiusBottomLeft <= 0 && radiusBottomRight <= 0)
            return;

        float offsetTL = MathF.Max(radiusTopLeft, thickness);
        float offsetTR = MathF.Max(radiusTopRight, thickness);
        float offsetBL = MathF.Max(radiusBottomLeft, thickness);
        float offsetBR = MathF.Max(radiusBottomRight, thickness);
        
        int innerTL = AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Top + thickness), color);
        int innerTR = AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Top + thickness), color);
        int innerBL = AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Bottom - thickness), color);
        int innerBR = AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Bottom - thickness), color);
        int innerTL2 = AddVertex(new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top + thickness), color);
        int innerTR2 = AddVertex(new Vector2(rectangle.Right - radiusTopRight, rectangle.Top + thickness), color);
        int innerBL2 = AddVertex(new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom - thickness), color);
        int innerBR2 = AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom - thickness), color);
        int innerTL3 = AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Top + radiusTopLeft), color);
        int innerTR3 = AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Top + radiusTopRight), color);
        int innerBL3 = AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Bottom - radiusBottomLeft), color);
        int innerBR3 = AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Bottom - radiusBottomRight), color);

        int outerTL1 = AddVertex(new Vector2(rectangle.Left, rectangle.Top + radiusTopLeft), color);
        int outerTL2 = AddVertex(new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top), color);
        int outerTR1 = AddVertex(new Vector2(rectangle.Right, rectangle.Top + radiusTopRight), color);
        int outerTR2 = AddVertex(new Vector2(rectangle.Right - radiusTopRight, rectangle.Top), color);
        int outerBL1 = AddVertex(new Vector2(rectangle.Left, rectangle.Bottom - radiusBottomLeft), color);
        int outerBL2 = AddVertex(new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom), color);
        int outerBR1 = AddVertex(new Vector2(rectangle.Right, rectangle.Bottom - radiusBottomRight), color);
        int outerBR2 = AddVertex(new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom), color);

        Vector2 cTL = new Vector2(rectangle.Left + radiusTopLeft, rectangle.Top + radiusTopLeft);
        Vector2 cTR = new Vector2(rectangle.Right - radiusTopRight, rectangle.Top + radiusTopRight);
        Vector2 cBL = new Vector2(rectangle.Left + radiusBottomLeft, rectangle.Bottom - radiusBottomLeft);
        Vector2 cBR = new Vector2(rectangle.Right - radiusBottomRight, rectangle.Bottom - radiusBottomRight);
        
        AddQuad(outerTL1, innerTL3, outerBL1, innerBL3);
        AddQuad(outerTL2, outerTR2, innerTL2, innerTR2);
        AddQuad(innerTR3, outerTR1, innerBR3, outerBR1);
        AddQuad(innerBL2, innerBR2, outerBL2, outerBR2);
        
        AddRoundedRectangleOutlineCurve(cTL, innerTL, thickness, radiusTopLeft, -1, -1, color);
        AddRoundedRectangleOutlineCurve(cTR, innerTR, thickness, radiusTopRight, 1, -1, color);
        AddRoundedRectangleOutlineCurve(cBL, innerBL, thickness, radiusBottomLeft, -1, 1, color);
        AddRoundedRectangleOutlineCurve(cBR, innerBR, thickness, radiusBottomRight, 1, 1, color);
    }

    private void AddRoundedRectangleOutlineCurve(Vector2 center, int innerCorner, float thickness, float radius, float directionH, float directionV, Color color)
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
            
            int nextInner = AddVertex(new Vector2(xInner, yInner), color);
            int nextOuter = AddVertex(new Vector2(xOuter, yOuter), color);

            if (i > 0)
            {
                AddTriangle(currentInner, currentOuter, nextInner);
                AddTriangle(currentOuter, nextInner, nextOuter);
            }

            if (connectToInnerCorner)
            {
                if (i == 0 || i == segments - 1)
                {
                    AddTriangle(nextOuter, nextInner, innerCorner);
                }
            }
            
            currentInner = nextInner;
            currentOuter = nextOuter;
        }
    }
}