using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public struct GuiMesh
{
    public readonly VertexPositionColorTexture[] Vertices;
    public readonly int[] Indices;

    public GuiMesh(VertexPositionColorTexture[] vertices, int[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }
}