using FontStashSharp.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public sealed class GuiMeshBuilder
{
    private readonly List<VertexPositionColorTexture> vertices = new();
    private readonly List<int> indices = new();
    private readonly Texture2D? texture;

    public GuiMeshBuilder(Texture2D? texture)
    {
        this.texture = texture;
    }

    public VertexPositionColorTexture this[int index]
    {
        get => vertices[index];
        set => vertices[index] = value;
    }
    
    public GuiSubMesh ExportSubMesh()
    {
        return new GuiSubMesh(vertices.ToArray(), indices.ToArray(), texture);
    }
    
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
        AddTriangle(c, b, d);
    }
}