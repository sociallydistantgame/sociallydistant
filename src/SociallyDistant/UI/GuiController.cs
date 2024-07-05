using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI;
using SociallyDistant.UI.Common;

namespace SociallyDistant.UI;

public class GuiController : GameComponent
{
    private readonly FlexPanel mainPanel = new();
    private readonly StatusBar statusBar = new();
    private readonly Box mainBox = new();
    
    private readonly GuiService guiService;
    
    public GuiController(IGameContext game) : base(game.GameInstance)
    {
        game.GameInstance.MustGetComponent(out guiService);
        game.GameModeObservable.Subscribe(OnGameModeChanged);

        guiService.GuiRoot.TopLevels.Add(mainPanel);

        mainPanel.ChildWidgets.Add(statusBar);
        mainPanel.ChildWidgets.Add(mainBox);

        mainBox.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
    }

    public async Task ShowExceptionMessage(Exception ex)
    {
        // TODO
    }

    private void OnGameModeChanged(GameMode gameMode)
    {
        
    }
}