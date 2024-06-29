using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Config.SystemConfigCategories;
using SociallyDistant.Core.Core.Config;

namespace SociallyDistant.DevTools;

public sealed class SettingsDebugMenu : IDevMenu
{
    private readonly ISettingsManager settingsManager;
    private readonly string[] resolutions;
		
    public string Name => "Change Settings";

    public SettingsDebugMenu()
    {
        settingsManager = SociallyDistantGame.Instance.SettingsManager;
        resolutions = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.OrderByDescending(x => x.Width * x.Height)
            .Select(x => $"{x.Width}x{x.Height}")
            .Distinct()
            .ToArray();
        
        
    }
		
    public void OnMenuGUI(DeveloperMenu devMenu)
    {
        var graphicsSettings = new GraphicsSettings(settingsManager);

        bool fullscreen = graphicsSettings.Fullscreen;

        ImGui.Checkbox("Fullscreen", ref fullscreen);
			
        ImGui.Text("Set Resolution");

        graphicsSettings.Fullscreen = fullscreen;

        foreach (string resolution in resolutions)
        {
            if (ImGui.Button(resolution))
                graphicsSettings.DisplayResolution = resolution;
        }
    }
}