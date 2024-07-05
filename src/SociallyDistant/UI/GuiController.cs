using Microsoft.Xna.Framework;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI;

namespace SociallyDistant.UI;

public class GuiController : GameComponent
{
    private readonly GuiService guiService;
    
    public GuiController(IGameContext game) : base(game.GameInstance)
    {
        game.GameInstance.MustGetComponent(out guiService);
    }

    public async Task ShowExceptionMessage(Exception ex)
    {
        // TODO
    }
}