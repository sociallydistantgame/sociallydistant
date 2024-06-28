using AcidicGUI;
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
    private readonly Box test = new();
    private SpriteEffect? defaultEffect;
    private Texture2D? white = null;

    public GuiService(IGameContext sociallyDistantContext) : base(sociallyDistantContext.GameInstance)
    {
        this.context = sociallyDistantContext;
        this.acidicGui = new GuiManager(this);
        this.acidicGui.TopLevels.Add(test);
    }

    public override void Update(GameTime gameTime)
    {
        acidicGui.UpdateLayout();
    }

    public override void Draw(GameTime gameTime)
    {
        acidicGui.Render();
    }

    public float PhysicalScreenWidget => Game.GraphicsDevice.Viewport.Width;
    public float PhysicalScreenHeight => Game.GraphicsDevice.Viewport.Height;
    
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

        graphics.Textures[0] = texture ?? white;
        defaultEffect.Techniques[0].Passes[0].Apply();
        graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, vertices, 0,
            vertices.Length, indices, 0, indices.Length / 3);
    }
}