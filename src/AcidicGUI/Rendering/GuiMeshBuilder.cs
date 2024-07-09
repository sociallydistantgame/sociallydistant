using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public sealed class GuiMeshBuilder
{
    private readonly List<VertexPositionColorTexture> vertices = new();
    private readonly List<int>                        indices  = new();
    private readonly Texture2D?                       texture;
    private readonly bool                             desaturate;
    private readonly int                              baseVertex;
    private readonly float                            layer;

    public GuiMeshBuilder(Texture2D? texture, int baseVertex, int layer, bool desaturate)
    {
        this.texture = texture;
        this.desaturate = desaturate;
            //this.baseVertex = baseVertex;
            //this.layer = layer * float.Epsilon;
    }

    public VertexPositionColorTexture this[int index]
    {
        get => vertices[index];
    }
    
    public GuiSubMesh ExportSubMesh()
    {
        return new GuiSubMesh(vertices.ToArray(), indices.ToArray(), texture);
    }
    
    public int AddVertex(VertexPositionColorTexture vertex)
    {
        int index = vertices.Count;

        vertex.Position.Z = layer;

        if (desaturate)
            vertex.Color.A = (byte) (vertex.Color.A / 2);
        
        vertices.Add(vertex);
        return index;
    }
    
    public int AddVertex(Vector3 position, Color color, Vector2 texCoord)
    {
        return AddVertex(new VertexPositionColorTexture(position, color, texCoord));
    }
    
    public int AddVertex(Vector2 position, Color color, Vector2 texCoord)
    {
        return AddVertex(new VertexPositionColorTexture(new Vector3(position.X, position.Y, 0), color, texCoord));
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
        indices.Add(a + baseVertex);
        indices.Add(b + baseVertex);
        indices.Add(c + baseVertex);
    }

    public void AddQuad(int a, int b, int c, int d)
    {
        AddTriangle(a, b, c);
        AddTriangle(c, b, d);
    }
}