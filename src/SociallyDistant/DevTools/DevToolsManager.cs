using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;

namespace SociallyDistant.DevTools;

public class DevToolsManager : DrawableGameComponent
{
    private readonly DeveloperMenu devMenu;
    
    private ImGuiRenderer? guiRenderer;
    
    public DevToolsManager(Game game) : base(game)
    {
        devMenu = new DeveloperMenu();
    }

    public override void Initialize()
    {
        devMenu.Initialize();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        guiRenderer = new ImGuiRenderer(Game);
        guiRenderer.RebuildFontAtlas();
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.OemTilde))
            devMenu.ToggleVisible();
        
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        if (guiRenderer == null)
            return;
        
        guiRenderer.BeginLayout(gameTime);

        devMenu.OnGUI();
        
        guiRenderer.EndLayout();
    }
}