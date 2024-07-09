using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Modules;

namespace SociallyDistant;

public sealed class VirtualScreen : IVirtualScreen
{
    private readonly GraphicsDevice  graphicsDevice;
    private readonly BasicEffect     blitEffect;
    private readonly VertexBuffer    quadVerts;
    private readonly IndexBuffer     quad;
    private          RenderTarget2D? virtualScreenTarget;
    private          int             width;
    private          int             height;
    private          bool            isActive;

    public VirtualScreen(GraphicsDevice graphicsDevice)
    {
        this.graphicsDevice = graphicsDevice;
        this.blitEffect = new BasicEffect(graphicsDevice);
        this.quadVerts = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), 4, BufferUsage.None);
        this.quad = new IndexBuffer(graphicsDevice, typeof(int), 6, BufferUsage.None);
		
        quadVerts.SetData(new VertexPositionColorTexture[] { new VertexPositionColorTexture(new Vector3(-1, -1, 0), Color.White, new Vector2(0, 1)), new VertexPositionColorTexture(new Vector3(1, -1, 0), Color.White, new Vector2(1, 1)), new VertexPositionColorTexture(new Vector3(-1, 1, 0), Color.White, new Vector2(0, 0)), new VertexPositionColorTexture(new Vector3(1, 1, 0), Color.White, new Vector2(1, 0)) });
        quad.SetData(new int[] { 0, 1, 2, 2, 1, 3 });
    }

    public void SetSize(int newWidth, int newHeight)
    {
        this.width = newWidth;
        this.height = newHeight;
    }
	
    public void Activate()
    {
        if (virtualScreenTarget == null || virtualScreenTarget.Width != width || virtualScreenTarget.Height != height)
        {
            Deactivate();
            virtualScreenTarget?.Dispose();
            virtualScreenTarget = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 4, RenderTargetUsage.PreserveContents);
        }
        
        isActive = true;
        graphicsDevice.SetRenderTarget(virtualScreenTarget);
    }

    public void Deactivate()
    {
        isActive = false;
        graphicsDevice.SetRenderTarget(null);
    }

    public void Blit(RenderTarget2D? target = null)
    {
        bool wasActive = isActive;
        if (wasActive)
            Deactivate();

        graphicsDevice.SetRenderTarget(target);
        graphicsDevice.Clear(Color.Black);

        blitEffect.TextureEnabled = true;
        blitEffect.CurrentTechnique.Passes[0].Apply();
        graphicsDevice.Textures[0] = virtualScreenTarget;
        graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
        graphicsDevice.BlendState = BlendState.AlphaBlend;
        graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.SetVertexBuffer(quadVerts);
        graphicsDevice.Indices = quad;
        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
		
        if (wasActive)
            Activate();
    }
}