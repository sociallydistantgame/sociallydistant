using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.InfoPanel;
using SociallyDistant.Core.UI;
using SociallyDistant.UI.Common;
using SociallyDistant.UI.InfoWidgets;
using SociallyDistant.UI.Shell;

namespace SociallyDistant.UI;

public class GuiController : 
    GameComponent,
    IShellContext
{
    private readonly NotificationManager notificationManager;
    private readonly InfoPanelController infoPanel = new();
    private readonly FlexPanel mainPanel = new();
    private readonly StatusBar statusBar = new();
    private readonly Box mainBox = new();
    private readonly GuiService guiService;
    
    public GuiController(IGameContext game) : base(game.GameInstance)
    {
        game.GameInstance.MustGetComponent(out guiService);
        game.GameModeObservable.Subscribe(OnGameModeChanged);

        notificationManager = new NotificationManager(game.WorldManager);
        
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

    public INotificationManager NotificationManager => notificationManager;
    public IInfoPanelService InfoPanelService => infoPanel;

    public async Task ShowInfoDialog(string title, string message)
    {
        // TODO
    }
}