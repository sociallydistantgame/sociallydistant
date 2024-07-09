using AcidicGUI.Effects;
using AcidicGUI.Rendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.Core.UI.Effects;

public sealed class BackgroundBlurWidgetEffect : IWidgetEffect,
    IDisposable
{
    private static   BackgroundBlurWidgetEffect? instance;
    private readonly MonoGameEffect              defaultUiShader;
    private readonly Effect                      gaussianShader;
    private          float                       blurAmount;
    private          RenderTarget2D?             blurTarget1;
    private          RenderTarget2D?             blurTarget2;
    private readonly VertexBuffer                vertexBuffer;
    private readonly IndexBuffer                 indexBuffer;
    private readonly EffectParameter             blurAmountParameter;
    private readonly EffectParameter             texelSizeParameter;
    private          Vector2                     texelSize = new Vector2(0, 0);
    private          Vector2                     widgetPosition;
    private          Vector2                     widgetSize;

    private readonly BlendState blendState = new BlendState
    {
        ColorSourceBlend = Blend.SourceAlpha, ColorDestinationBlend = Blend.InverseSourceAlpha, AlphaSourceBlend = Blend.One, AlphaDestinationBlend = Blend.InverseSourceAlpha,
    };

    private BackgroundBlurWidgetEffect(MonoGameEffect DEFAULTuIsHADER, Effect BLUReFFECT)
    {
        this.defaultUiShader = DEFAULTuIsHADER;
        this.gaussianShader = BLUReFFECT;

        blurAmountParameter = BLUReFFECT.Parameters["BlurAmount"];
        texelSizeParameter = BLUReFFECT.Parameters["TexelSize"];
        
        vertexBuffer = new VertexBuffer(BLUReFFECT.GraphicsDevice, typeof(VertexPositionColorTexture), 4, BufferUsage.None);
        indexBuffer = new IndexBuffer(BLUReFFECT.GraphicsDevice, typeof(int), 6, BufferUsage.None);

        vertexBuffer.SetData(new VertexPositionColorTexture[] { new VertexPositionColorTexture(new Vector3(-1, -1, 0), Color.White, new Vector2(0, 0)), new VertexPositionColorTexture(new Vector3(1, -1, 0), Color.White, new Vector2(1, 0)), new VertexPositionColorTexture(new Vector3(-1, 1, 0), Color.White, new Vector2(0, 1)), new VertexPositionColorTexture(new Vector3(1, 1, 0), Color.White, new Vector2(1, 1)) });
        indexBuffer.SetData(new int[] { 0, 1, 2, 2, 1, 3 });
    }
    
    public void Dispose()
    {
        gaussianShader.Dispose();
    }

    public int PassesCount => defaultUiShader.PassesCount;
    public void Use(int pass)
    {
        defaultUiShader.Use(pass);
    }

    public void UpdateOpacity(float opacity)
    {
        defaultUiShader.UpdateOpacity(opacity);
    }
    
    public void UpdateParameters(Widget widget, GuiRenderer renderer)
    {
        var blurSettings = widget.GetCustomProperties<BackgroundBlurProperties>();

        blurAmount = blurSettings.BlurAmount;

        Viewport viewport = renderer.GraphicsDevice.Viewport;

        ResizeIfNeeded(ref blurTarget1, viewport.Width, viewport.Height);
        ResizeIfNeeded(ref blurTarget2, viewport.Width, viewport.Height);

        texelSize.X = 1f / viewport.Width;
        texelSize.Y = 1f / viewport.Height;
        
        renderer.Grab(blurTarget1);

        widgetPosition = widget.ContentArea.TopLeft;
        widgetSize = widget.ContentArea.Size;
        
        DoBlur(blurTarget1, blurTarget2, 0);
        DoBlur(blurTarget2, blurTarget1, 1);

        renderer.Restore();
    }

    public void BeforeRebuildGeometry(GeometryHelper geometry)
    {
        var mesh = geometry.GetMeshBuilder(blurTarget1);

        int v1 = mesh.AddVertex(new Vector2(widgetPosition.X,                widgetPosition.Y),                Color.White, new Vector2(texelSize.X * widgetPosition.X,                  texelSize.Y * widgetPosition.Y));
        int v2 = mesh.AddVertex(new Vector2(widgetPosition.X + widgetSize.X, widgetPosition.Y),                Color.White, new Vector2(texelSize.X * (widgetPosition.X + widgetSize.X), texelSize.Y * widgetPosition.Y));
        int v3 = mesh.AddVertex(new Vector2(widgetPosition.X,                widgetPosition.Y + widgetSize.Y), Color.White, new Vector2(texelSize.X * widgetPosition.X,                  texelSize.Y * (widgetPosition.Y + widgetSize.Y)));
        int v4 = mesh.AddVertex(new Vector2(widgetPosition.X + widgetSize.X, widgetPosition.Y + widgetSize.Y), Color.White, new Vector2(texelSize.X * (widgetPosition.X + widgetSize.X), texelSize.Y * (widgetPosition.Y + widgetSize.Y)));

        mesh.AddQuad(v1, v2, v3, v4);
    }

    public void AfterRebuildGeometry(GeometryHelper geometry)
    {
    }

    private void ResizeIfNeeded(ref RenderTarget2D? target, int width, int height)
    {
        if (target == null || target.Width != width || target.Height != height)
        {
            target?.Dispose();
            target = new RenderTarget2D(gaussianShader.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 4, RenderTargetUsage.PreserveContents);
        }
    }

    private void DoBlur(RenderTarget2D source, RenderTarget2D destination, int pass)
    {
        gaussianShader.GraphicsDevice.SetRenderTarget(destination);
        gaussianShader.GraphicsDevice.Clear(Color.Black);

        gaussianShader.Techniques[0].Passes[pass].Apply();

        blurAmountParameter.SetValue(blurAmount);
        texelSizeParameter.SetValue(texelSize);

        gaussianShader.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        gaussianShader.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        gaussianShader.GraphicsDevice.BlendState = BlendState.Opaque;
        gaussianShader.GraphicsDevice.SetVertexBuffer(vertexBuffer);
        gaussianShader.GraphicsDevice.Indices = indexBuffer;
        gaussianShader.GraphicsDevice.Textures[0] = source;
        gaussianShader.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
    }

    public static BackgroundBlurWidgetEffect GetEffect(IGameContext context)
    {
        if (instance != null)
            return instance;
        
        Effect blur = context.GameInstance.Content.Load<Effect>("/Core/Shaders/GaussianBlur");
        MonoGameEffect uiShader = new MonoGameEffect(context.GameInstance.Content.Load<Effect>("/Core/Shaders/UI_Core"));
        
        instance = new BackgroundBlurWidgetEffect(uiShader, blur);
        return instance;
    }
}