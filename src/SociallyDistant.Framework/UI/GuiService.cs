using AcidicGUI;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.Core.UI;

public sealed class GuiService : 
    DrawableGameComponent,
    IGuiContext
{
    private readonly IGameContext context;
    private readonly GuiManager acidicGui;
    private readonly IGuiContext guiContext;
    private readonly FlexPanel test = new();
    private readonly int[] screenQuad = new int[] { 0, 1, 2, 2, 1, 3 };
    private readonly VertexPositionColorTexture[] screenQuadVerts = new VertexPositionColorTexture[4];
    private SpriteEffect? defaultEffect;
    private Texture2D? white = null;
    private RenderTarget2D? virtualScreen;

    public GuiService(IGameContext sociallyDistantContext) : base(sociallyDistantContext.GameInstance)
    {
        this.context = sociallyDistantContext;
        this.acidicGui = new GuiManager(this);
        this.acidicGui.TopLevels.Add(test);
        
        test.Spacing = 6;
        test.Direction = Direction.Vertical;
    }

    public void SetVirtualScreenSize(int width, int height)
    {
        virtualScreen?.Dispose();
        virtualScreen = new RenderTarget2D(Game.GraphicsDevice, width, height, false, SurfaceFormat.Rgba64, DepthFormat.Depth24Stencil8);

        int physicalWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        int physicalHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
        
        screenQuadVerts[0].Position = new Vector3(0, 0, 0);
        screenQuadVerts[0].Color = Color.White;
        screenQuadVerts[0].TextureCoordinate = new Vector2(0, 0);
        
        screenQuadVerts[1].Position = new Vector3(physicalWidth, 0, 0);
        screenQuadVerts[1].Color = Color.White;
        screenQuadVerts[1].TextureCoordinate = new Vector2(1, 0);
        
        screenQuadVerts[2].Position = new Vector3(0, physicalHeight, 0);
        screenQuadVerts[2].Color = Color.White;
        screenQuadVerts[2].TextureCoordinate = new Vector2(0, 1);
        
        screenQuadVerts[3].Position = new Vector3(physicalWidth, physicalHeight, 0);
        screenQuadVerts[3].Color = Color.White;
        screenQuadVerts[3].TextureCoordinate = new Vector2(1, 1);
    }
    
    public override void Update(GameTime gameTime)
    {
        acidicGui.UpdateLayout();
    }

    public override void Draw(GameTime gameTime)
    {
        if (virtualScreen == null)
            return;
        
        Game.GraphicsDevice.SetRenderTarget(virtualScreen);
        Game.GraphicsDevice.Clear(Color.Transparent);
        
        acidicGui.Render();
        
        Game.GraphicsDevice.SetRenderTarget(null);

        Render(screenQuadVerts, screenQuad, virtualScreen);
    }

    public float PhysicalScreenWidget => virtualScreen?.Width ?? Game.GraphicsDevice.Viewport.Width;
    public float PhysicalScreenHeight => virtualScreen?.Height ?? Game.GraphicsDevice.Viewport.Height;
    
    public void Render(VertexPositionColorTexture[] vertices, int[] indices, Texture2D? texture)
    {
        if (defaultEffect == null)
        {
            defaultEffect = new SpriteEffect(Game.GraphicsDevice);
        }

        if (white == null)
        {
            white = new Texture2D(Game.GraphicsDevice, 1, 1);
            white.SetData(new Color[] { Color.White });
        }
        
        if (vertices.Length == 0)
            return;

        if (indices.Length == 0)
            return;
        
        var graphics = Game.GraphicsDevice;

        defaultEffect.Techniques[0].Passes[0].Apply();
        graphics.Textures[0] = texture ?? white;
        graphics.SamplerStates[0] = SamplerState.LinearClamp;
        graphics.BlendState = BlendState.AlphaBlend;
        graphics.RasterizerState = RasterizerState.CullCounterClockwise;
        graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, vertices, 0,
            vertices.Length, indices, 0, indices.Length / 3);
    }
}