using System.Text;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.InfoPanel;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI;
using SociallyDistant.Player;
using SociallyDistant.UI.Common;
using SociallyDistant.UI.InfoWidgets;
using SociallyDistant.UI.Shell;
using SociallyDistant.UI.Windowing;

namespace SociallyDistant.UI;

public class GuiController : 
    GameComponent,
    IShellContext
{
    private readonly NotificationManager notificationManager;
    private readonly InfoPanelController infoPanel = new();
    private readonly FlexPanel mainPanel = new();
    private readonly StatusBar statusBar = new();
    private readonly OverlayWidget workArea = new();
    private readonly Box mainBox = new();
    private readonly FloatingWorkspace floatingWindowArea = new();
    private readonly GuiService guiService;
    private readonly PlayerManager playerManager;
    private readonly DesktopController desktopController;

    private Desktop? desktop;
    
    public StatusBar StatusBar => statusBar;
    
    public GuiController(IGameContext game, PlayerManager playerManager) : base(game.GameInstance)
    {
        this.playerManager = playerManager;

        this.desktopController = new DesktopController(this, this.playerManager);
        
        game.GameInstance.MustGetComponent(out guiService);
        game.GameModeObservable.Subscribe(OnGameModeChanged);

        notificationManager = new NotificationManager(game.WorldManager);
        
        guiService.GuiRoot.TopLevels.Add(mainPanel);

        mainPanel.ChildWidgets.Add(statusBar);
        mainPanel.ChildWidgets.Add(workArea);

        workArea.ChildWidgets.Add(mainBox);
        workArea.ChildWidgets.Add(floatingWindowArea);
        
        workArea.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
    }

    public Task ShowExceptionMessage(Exception ex)
    {
        var dialog = CreateMessageDialog("System Error");
        var completionSource = new TaskCompletionSource<MessageDialogResult>();

        var messageBuilder = new StringBuilder();

        messageBuilder.AppendLine("Socially Distant has encountered an unexpected problem.");
        messageBuilder.AppendLine();
        messageBuilder.AppendLine("<b>What happened?</b>");
        messageBuilder.AppendLine(
            $"An unhandled .NET runtime exception ({ex.GetType().FullName}) occurred. {ex.Message}");

        messageBuilder.AppendLine();
        messageBuilder.AppendLine(
            "This generally happens as a result of a broken mod or a bug in the game. Details have been logged to the game's log.");
        
        Log.Error(ex.ToString());

        dialog.Message = messageBuilder.ToString();
        dialog.Buttons.Add("OK");
        
        return completionSource.Task;
    }

    private void OnGameModeChanged(GameMode gameMode)
    {
        if (gameMode == GameMode.OnDesktop)
        {
            this.desktop = new Desktop(desktopController);
            this.mainBox.Content = desktop;
            
            desktopController.Login();
        }
        else
        {
            desktop?.Dispose();
        }
    }

    public INotificationManager NotificationManager => notificationManager;
    public IInfoPanelService InfoPanelService => infoPanel;

    public Task ShowInfoDialog(string title, string message)
    {
        var completionSource = new TaskCompletionSource<MessageDialogResult>();
        var dialog = CreateMessageDialog(title);

        dialog.Message = message;
        dialog.DismissCallback = completionSource.SetResult;
        dialog.Buttons.Add("OK");

        return completionSource.Task;
    }

    private OverlayWorkspace CreateOverlayWorkspace()
    {
        var workspace = new OverlayWorkspace();
        this.workArea.ChildWidgets.Add(workspace);
        return workspace;
    }
    
    public IMessageDialog CreateMessageDialog(string title)
    {
        var overlay = CreateOverlayWorkspace();
        var window = overlay.CreateWindow(title);
        var dialog = new MessageDialog(window);

        dialog.Title = title;
        
        dialog.WindowClosed += HandleDialogClosed;

        return dialog;
        
        void HandleDialogClosed(IWindow obj)
        {
            workArea.ChildWidgets.Remove(overlay);
        }
    }
}

internal sealed class DesktopController
{
    private readonly PlayerManager playerManager;
    private readonly GuiController guiController;
    
    private IUser? loginUser;
    private ISystemProcess loginProcess;
    
    
    public DesktopController(GuiController gui, PlayerManager player)
    {
        this.guiController = gui;
        this.playerManager = player;
    }

    public void Login()
    {
        loginUser = playerManager.PlayerUser;
        loginProcess = playerManager.InitProcess.CreateLoginProcess(loginUser);

        guiController.StatusBar.User = loginUser;
    }
}