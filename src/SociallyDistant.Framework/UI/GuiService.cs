using AcidicGUI;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Rendering;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.Core.UI;

public sealed class GuiService : 
    DrawableGameComponent,
    IGuiContext
{
    private readonly IGameContext context;
    private readonly GuiManager acidicGui;
    private readonly IGuiContext guiContext;
    private readonly ScrollView test = new();
    private readonly int[] screenQuad = new int[] { 0, 1, 2, 2, 1, 3 };
    private readonly VertexPositionColorTexture[] screenQuadVerts = new VertexPositionColorTexture[4];
    private readonly SociallyDistantVisualStyle visualStyle;
    private Font? fallbackFont;
    private SpriteEffect? defaultEffect;
    private Texture2D? white = null;
    private RenderTarget2D? virtualScreen;
    private RasterizerState? scissor;
    private RasterizerState? noScissor;

    public GuiService(IGameContext sociallyDistantContext) : base(sociallyDistantContext.GameInstance)
    {
        visualStyle = new SociallyDistantVisualStyle(sociallyDistantContext);
        this.context = sociallyDistantContext;
        this.acidicGui = new GuiManager(this, visualStyle);
        this.acidicGui.TopLevels.Add(test);
        
        test.HorizontalAlignment = HorizontalAlignment.Center;
        test.VerticalAlignment = VerticalAlignment.Middle;
        test.MaximumSize = new Vector2(1280, 720);
        test.Spacing = 6;
        test.Padding = 12;

        var inputField = new InputField();
        inputField.WordWrapped = true;
        inputField.MultiLine = true;
        
        test.ChildWidgets.Add(inputField);
        
        for (var i = 0; i < 36; i++)
        {
            var button = new Button();
            var text = new TextWidget();

            text.Text =
                $"Ritchie {i + 1} - <color=green>This text should be green</color> and <highlight=red>this text should be highlighted red</highlight>";
            text.HorizontalAlignment = HorizontalAlignment.Stretch;
            text.VerticalAlignment = VerticalAlignment.Middle;
            text.WordWrapping = true;

            button.Content = text;
            test.ChildWidgets.Add(button);
        }
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
        if (virtualScreen == null)
            return;

        // TODO: Render the entire game to a virtual screen so we can do background-blurs
        Game.GraphicsDevice.SetRenderTarget(virtualScreen);
        Game.GraphicsDevice.Clear(Color.Transparent);
        acidicGui.Render();
        Game.GraphicsDevice.SetRenderTarget(null);
        
        Render(screenQuadVerts, screenQuad, virtualScreen);
    }

    public float PhysicalScreenWidget => virtualScreen?.Width ?? Game.GraphicsDevice.Viewport.Width;
    public float PhysicalScreenHeight => virtualScreen?.Height ?? Game.GraphicsDevice.Viewport.Height;
    
    public void Render(VertexPositionColorTexture[] vertices, int[] indices, Texture2D? texture, LayoutRect? clipRect = null)
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
        graphics.RasterizerState = GetRasterizerState(clipRect);
        graphics.ScissorRectangle = clipRect.GetValueOrDefault();
        graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, vertices, 0,
            vertices.Length, indices, 0, indices.Length / 3);
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

    public Font GetFallbackFont()
    {
        if (fallbackFont == null)
            fallbackFont = Game.Content.Load<SpriteFont>("/Core/UI/Fonts/Rajdhani");

        return fallbackFont;
    }
}