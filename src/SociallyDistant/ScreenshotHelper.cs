using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Serilog;
using SociallyDistant.Core.Core.Scripting;

namespace SociallyDistant;

public sealed class ScreenshotHelper : GameComponent
{
    private readonly SociallyDistantGame game;
    private readonly VirtualScreen       screen;
    private readonly string              screenshotsPath;
    private          bool                wasScreenshotKeyPressed;
    
    internal ScreenshotHelper(SociallyDistantGame game, VirtualScreen screen, string gameDataPath) : base(game)
    {
        this.game = game;
        this.screen = screen;
        this.screenshotsPath = Path.Combine(gameDataPath, "screenshots");
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        bool isScreenshotKeyPressed = keyboard.IsKeyDown(Keys.F2);

        if (isScreenshotKeyPressed && !wasScreenshotKeyPressed)
        {
            TakeScreenshot();
        }

        wasScreenshotKeyPressed = isScreenshotKeyPressed;
    }

    private void TakeScreenshot()
    {
        if (!Directory.Exists(screenshotsPath))
            Directory.CreateDirectory(screenshotsPath);

        var utcNow = DateTime.UtcNow;
        var filename = $"{utcNow.Year}-{utcNow.Month}-{utcNow.Day} {utcNow.Hour}-{utcNow.Minute}-{utcNow.Second}.{utcNow.Millisecond}.png";

        using var stream = File.OpenWrite(Path.Combine(screenshotsPath, filename));

        Log.Information($"Saving screenshot to {filename}...");
        this.game.VirtualScreen.SaveScreenshot(stream);
    }
}