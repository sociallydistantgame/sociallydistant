using AcidicGUI.Layout;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public struct GuiSubMesh
{
    public readonly VertexPositionColorTexture[] Vertices;
    public readonly int[] Indices;
    public readonly Texture2D? Texture;

    private bool isNewMesh = true;

    public bool IsNew => isNewMesh;
    
    public GuiSubMesh(VertexPositionColorTexture[] vertices, int[] indices, Texture2D? texture)
    {
        Vertices = vertices;
        Indices = indices;
        this.Texture = texture;
        isNewMesh = true;
    }

    public void MarkAsOld()
    {
        isNewMesh = false;
    }
}

public struct GuiMesh
{
    public readonly GuiSubMesh[] SubMeshes;
    public readonly LayoutRect? ClipRect;

    public GuiMesh(GuiSubMesh[] subMeshes, LayoutRect? clipRect)
    {
        this.SubMeshes = subMeshes;
        this.ClipRect = clipRect;
    }
}