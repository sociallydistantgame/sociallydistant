using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public sealed class GuiRenderer
{
    private readonly IGuiContext context;
    
    public GuiRenderer(IGuiContext context)
    {
        this.context = context;
    }
    
    public void RenderGuiMesh(GuiMesh mesh)
    {
        foreach (GuiSubMesh subMesh in mesh.SubMeshes)
        {
            context.Render(subMesh.Vertices, subMesh.Indices, subMesh.Texture, mesh.ClipRect);
        }
    }

    public GraphicsDevice GraphicsDevice => context.GraphicsDevice;
}