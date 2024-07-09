using AcidicGUI;
using AcidicGUI.CustomProperties;
using AcidicGUI.Effects;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI.Effects;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.Core.UI;

public sealed class GuiService : 
    DrawableGameComponent,
    IGuiContext
{
    private readonly BlendState blendState = new BlendState
    {
        ColorSourceBlend = Blend.SourceAlpha,
        ColorDestinationBlend = Blend.InverseSourceAlpha,
        AlphaSourceBlend = Blend.One,
        AlphaDestinationBlend = Blend.InverseSourceAlpha,
    };
    private readonly IGameContext                 context;
    private readonly GuiManager                   acidicGui;
    private readonly IGuiContext                  guiContext;
    private readonly int[]                        screenQuad      = new int[] { 0, 1, 2, 2, 1, 3 };
    private readonly VertexPositionColorTexture[] screenQuadVerts = new VertexPositionColorTexture[4];
    private readonly SociallyDistantVisualStyle   visualStyle;
    private          IFontFamily?                 fallbackFont;
    private          MonoGameEffect?              defaultEffect;
    private          Texture2D?                   white = null;
    private          RenderTarget2D?              virtualScreen;
    private          RasterizerState?             scissor;
    private          RasterizerState?             noScissor;

    public GuiManager GuiRoot => acidicGui;
    
    public GuiService(IGameContext sociallyDistantContext) : base(sociallyDistantContext.GameInstance)
    {
        visualStyle = new SociallyDistantVisualStyle(sociallyDistantContext);
        this.context = sociallyDistantContext;
        this.acidicGui = new GuiManager(this, visualStyle);
    }

    public void SetVirtualScreenSize(int width, int height)
    {
        virtualScreen?.Dispose();
        virtualScreen = new RenderTarget2D(Game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 4, RenderTargetUsage.PreserveContents);

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

    public override void Initialize()
    {
        base.Initialize();
        visualStyle.LoadContent();
        
        Game.Window.KeyDown += HandleKeyDown;
        Game.Window.KeyUp += HandleKeyUp;
        Game.Window.TextInput += HandleTextInput;
    }

    private void HandleTextInput(object? sender, TextInputEventArgs e)
    {
        acidicGui.SendCharacter(e.Key, e.Character);
    }

    private void HandleKeyUp(object? sender, InputKeyEventArgs e)
    {
        acidicGui.SendKey(e.Key, ButtonState.Released);
    }

    private void HandleKeyDown(object? sender, InputKeyEventArgs e)
    {
        acidicGui.SendKey(e.Key, ButtonState.Pressed);
    }

    public override void Update(GameTime gameTime)
    {
        // Handles layout updates
        acidicGui.UpdateLayout();
        
        // Mouse
        var mouse = Mouse.GetState(Game.Window);
        
        acidicGui.SetMouseState(mouse);
    }

    public override void Draw(GameTime gameTime)
    {
        acidicGui.Render();
    }

    public float PhysicalScreenWidget => virtualScreen?.Width ?? Game.GraphicsDevice.Viewport.Width;
    public float PhysicalScreenHeight => virtualScreen?.Height ?? Game.GraphicsDevice.Viewport.Height;

    private MonoGameEffect LoadDefaultEffect()
    {
        return new MonoGameEffect(Game.Content.Load<Effect>("/Core/Shaders/UI_Core"));
    }
    
    public void Render(VertexPositionColorTexture[] vertices, int[] indices, Texture2D? texture, LayoutRect? clipRect = null)
    {
        if (defaultEffect == null)
        {
            defaultEffect = LoadDefaultEffect();
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

        defaultEffect.Use(0);
        defaultEffect.UpdateOpacity(1);
        graphics.Textures[0] = texture ?? white;
        graphics.SamplerStates[0] = SamplerState.LinearClamp;
        graphics.BlendState = blendState;
        graphics.RasterizerState = GetRasterizerState(clipRect);
        graphics.ScissorRectangle = clipRect.GetValueOrDefault();
        graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, vertices, 0,
            vertices.Length, indices, 0, indices.Length / 3);
    }

    public void Render(
        VertexBuffer vertices,
        IndexBuffer indices,
        int offset,
        int primitiveCount,
        Texture2D? texture,
        LayoutRect? clipRect = null,
        IEffect? effectOverride = null,
        float opacity = 1
    )
    {
        var device = Game.GraphicsDevice;
        
        if (defaultEffect == null)
        {
            defaultEffect = LoadDefaultEffect();
        }

        if (white == null)
        {
            white = new Texture2D(Game.GraphicsDevice, 1, 1);
            white.SetData(new Color[] { Color.White });
        }
        
        if (primitiveCount == 0)
            return;

        IEffect effectToUse = effectOverride ?? defaultEffect;
        
        effectToUse.Use(0);
        effectToUse.UpdateOpacity(opacity);
        device.Textures[0] = texture ?? white;
        device.SamplerStates[0] = SamplerState.LinearClamp;
        device.BlendState = blendState;
        device.RasterizerState = GetRasterizerState(clipRect);
        device.DepthStencilState = DepthStencilState.DepthRead;
        device.ScissorRectangle = clipRect.GetValueOrDefault();
        
        device.SetVertexBuffer(vertices);
        device.Indices = indices;
        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, offset, primitiveCount);
    }

    public void Grab(RenderTarget2D destination)
    {
        context.VirtualScreen?.Blit(destination);
    }

    public void RestoreRenderState()
    {
        context.VirtualScreen?.Activate();
    }

    private RasterizerState GetRasterizerState(LayoutRect? clipRect)
    {
        scissor ??= new RasterizerState()
        {
            CullMode = CullMode.CullCounterClockwiseFace,
            ScissorTestEnable = true
        };
        noScissor ??= new RasterizerState()
        {
            CullMode = CullMode.CullCounterClockwiseFace,
            ScissorTestEnable = false
        };
        
        if (clipRect != null)
        {
            return scissor;
        }
        else
        {
            return noScissor;
        }
    }

    public IFontFamily GetFallbackFont()
    {
        if (fallbackFont == null)
            fallbackFont = new FontFamily(Game.Content.Load<SpriteFont>("/Core/UI/Fonts/Fallback"));

        return fallbackFont;
    }
}