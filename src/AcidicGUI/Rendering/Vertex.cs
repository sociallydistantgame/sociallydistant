using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public sealed class GuiBatcher
{
    private readonly List<VertexPositionColorTexture> batchedVertices = new();
    private readonly List<int> batchedIndices = new();
    private readonly IGuiContext context;

    public GuiBatcher(IGuiContext context)
    {
        this.context = context;
    }
    
    public void BatchGuiMesh(GuiMesh mesh)
    {
        int baseIndex = batchedVertices.Count;
        
        batchedVertices.AddRange(mesh.Vertices);

        foreach (var i in mesh.Indices)
            batchedIndices.Add(i + baseIndex);
    }

    internal void RenderBatch()
    {
        context.Render(batchedVertices.ToArray(), batchedIndices.ToArray(), null);
    }
}