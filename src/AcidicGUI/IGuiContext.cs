using System.Buffers;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI;

public interface IGuiContext
{
    float PhysicalScreenWidget { get; }
    float PhysicalScreenHeight { get; }

    void Render(VertexPositionColorTexture[] vertices, int[] indices, Texture2D? texture);
}