using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Social;

namespace SociallyDistant.DevTools;

public sealed class EmailMenu : IDevMenu
{
    private readonly ISocialService social = null!;

    public EmailMenu(ISocialService social)
    {
        this.social = social;
    }

    /// <inheritdoc />
    public string Name => "Email Menu";

    /// <inheritdoc />
    public void OnMenuGUI(DeveloperMenu devMenu)
    {
        var world = WorldManager.Instance;
    }
}