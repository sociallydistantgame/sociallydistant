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

        Vector3 cTL = vertices[innerTL].Position;
        Vector3 cTR = vertices[innerTR].Position;
        Vector3 cBL = vertices[innerBL].Position;
        Vector3 cBR = vertices[innerBR].Position;

        int tlLast = outerTL2;
        int trLast = outerTR2;
        var blLast = outerBL2;
        int brLast = outerBR2;

        for (var i = 0; i < segments; i++)
        {
            var t = (i / ((float) segments - 1f)) * MathF.PI * 0.5f;
            var x = MathF.Sin(t);
            var y = MathF.Cos(t);

            float xTL = x * -radiusTopLeft + cTL.X;
            float yTL = y * -radiusTopLeft + cTL.Y;
            float xTR = x * radiusTopRight + cTR.X;
            float yTR = y * -radiusTopRight + cTR.Y;
            float xBL = x * -radiusTopLeft + cBL.X;
            float yBL = y * radiusTopLeft + cBL.Y;
            float xBR = x * radiusTopRight + cBR.X;
            float yBR = y * radiusTopRight + cBR.Y;
            

            int tl = AddVertex(new Vector2(xTL, yTL), color);
            int tr = AddVertex(new Vector2(xTR, yTR), color);
            int bl = AddVertex(new Vector2(xBL, yBL), color);
            int br = AddVertex(new Vector2(xBR, yBR), color);
            
            AddTriangle(tlLast, tl, innerTL);
            AddTriangle(trLast, innerTR, tr);
            AddTriangle(blLast, bl, innerBL);
            AddTriangle(brLast, innerBR, br);
            
            
            tlLast = tl;
            trLast = tr;
            blLast = bl;
            brLast = br;
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
        
        radiusTopLeft = MathHelper.Clamp(radiusTopLeft, 0, smallerHalf);
        radiusBottomLeft = MathHelper.Clamp(radiusBottomLeft, 0, smallerHalf);
        radiusTopRight = MathHelper.Clamp(radiusTopRight, 0, smallerHalf);
        radiusBottomRight = MathHelper.Clamp(radiusBottomRight, 0, smallerHalf);

        
        if (radiusTopLeft <= 0 && radiusTopRight <= 0 && radiusBottomLeft <= 0 && radiusBottomRight <= 0)
            return;
        
        int innerTL = AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Top + thickness), color);
        int innerTR = AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Top + thickness), color);
        int innerBL = AddVertex(new Vector2(rectangle.Left + thickness, rectangle.Bottom - thickness), color);
        int innerBR = AddVertex(new Vector2(rectangle.Right - thickness, rectangle.Bottom - thickness), color);

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
        
        AddQuad(outerTL1, innerTL, outerBL1, innerBL);
        AddQuad(outerTL2, outerTR2, innerTL, innerTR);
        AddQuad(innerTR, outerTR1, innerBR, outerBR1);
        AddQuad(innerBL, innerBR, outerBL2, outerBR2);

        const int segments = 16;
        int currentOuterTL = outerTL1;
        int currentInnerTL = innerTL;
        int currentOuterTR = outerTR1;
        int currentInnerTR = innerTR;
        int currentOuterBL = outerBL1;
        int currentInnerBL = innerBL;
        int currentOuterBR = outerBR1;
        int currentInnerBR = innerBR;
        for (var i = 0; i < segments; i++)
        {
            var t = (i / ((float) segments - 1f)) * MathF.PI * 0.5f;
            var x = MathF.Sin(t);
            var y = MathF.Cos(t);

            var outerXTL = x * -radiusTopLeft + cTL.X;
            var outerYTL = y * -radiusTopLeft + cTL.Y;
            var innerXTL = x * (-radiusTopLeft + thickness) + cTL.X;
            var innerYTL = y * (-radiusTopLeft + thickness) + cTL.Y;
            var outerXTR = x * radiusTopRight + cTR.X;
            var outerYTR = y * -radiusTopRight + cTR.Y;
            var innerXTR = x * (radiusTopRight - thickness) + cTR.X;
            var innerYTR = y * (-radiusTopRight + thickness) + cTR.Y;
            
            var outerXBL = x * -radiusBottomLeft + cBL.X;
            var outerYBL = y * radiusBottomLeft + cBL.Y;
            var innerXBL = x * (-radiusBottomLeft + thickness) + cBL.X;
            var innerYBL = y * (radiusBottomLeft - thickness) + cBL.Y;
            var outerXBR = x * radiusBottomRight + cBR.X;
            var outerYBR = y * radiusBottomRight + cBR.Y;
            var innerXBR = x * (radiusBottomRight - thickness) + cBR.X;
            var innerYBR = y * (radiusBottomRight - thickness) + cBR.Y;

            int nextInnerTL = AddVertex(new Vector2(innerXTL, innerYTL), color);
            int nextOuterTL = AddVertex(new Vector2(outerXTL, outerYTL), color);
            int nextInnerTR = AddVertex(new Vector2(innerXTR, innerYTR), color);
            int nextOuterTR = AddVertex(new Vector2(outerXTR, outerYTR), color);
            int nextInnerBL = AddVertex(new Vector2(innerXBL, innerYBL), color);
            int nextOuterBL = AddVertex(new Vector2(outerXBL, outerYBL), color);
            int nextInnerBR = AddVertex(new Vector2(innerXBR, innerYBR), color);
            int nextOuterBR = AddVertex(new Vector2(outerXBR, outerYBR), color);

            if (i > 0)
            {
                AddTriangle(currentInnerTL, currentOuterTL, nextInnerTL);
                AddTriangle(currentOuterTL, nextInnerTL, nextOuterTL);
                AddTriangle(currentInnerTR, currentOuterTR, nextInnerTR);
                AddTriangle(currentOuterTR, nextInnerTR, nextOuterTR);
                
                AddTriangle(currentInnerBL, currentOuterBL, nextInnerBL);
                AddTriangle(currentOuterBL, nextInnerBL, nextOuterBL);
                AddTriangle(currentInnerBR, currentOuterBR, nextInnerBR);
                AddTriangle(currentOuterBR, nextInnerBR, nextOuterBR);
            }

            currentInnerTL = nextInnerTL;
            currentOuterTL = nextOuterTL;
            currentInnerTR = nextInnerTR;
            currentOuterTR = nextOuterTR;
            currentInnerBL = nextInnerBL;
            currentOuterBL = nextOuterBL;
            currentInnerBR = nextInnerBR;
            currentOuterBR = nextOuterBR;
        }
    }
}