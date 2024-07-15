using System.Diagnostics;
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
using SociallyDistant.UI.CharacterCreation;
using SociallyDistant.UI.Common;
using SociallyDistant.UI.InfoWidgets;
using SociallyDistant.UI.MainMenu;
using SociallyDistant.UI.Settings;
using SociallyDistant.UI.Shell;
using SociallyDistant.UI.Windowing;

namespace SociallyDistant.UI;

public class GuiController : GameComponent,
    IShellContext
{
    private readonly SociallyDistantGame context;
    private readonly NotificationManager notificationManager;
    private readonly FlexPanel           mainPanel = new();
    private readonly StatusBar           statusBar;
    private readonly OverlayWidget       workArea           = new();
    private readonly OverlayWidget       overlays           = new();
    private readonly Box                 mainBox            = new();
    private readonly FloatingWorkspace   floatingWindowArea = new();
    private readonly GuiService          guiService;
    private readonly PlayerManager       playerManager;
    private readonly DesktopController   desktopController;
    private readonly TrayModel           trayModel = new();

    private SystemSettingsController? systemSettings;
    private Desktop?                  desktop;

    public StatusBar StatusBar => statusBar;
    public IGameContext Context => context;

    internal GuiController(SociallyDistantGame game, PlayerManager playerManager) : base(game.GameInstance)
    {
        this.context = game;
        
        statusBar = new StatusBar(trayModel);
        
        this.playerManager = playerManager;

        this.desktopController = new DesktopController(this, this.playerManager);

        game.GameInstance.MustGetComponent(out guiService);
        game.GameModeObservable.Subscribe(OnGameModeChanged);

        notificationManager = new NotificationManager(game.WorldManager);

        guiService.GuiRoot.TopLevels.Add(mainPanel);
        guiService.GuiRoot.TopLevels.Add(overlays);
        
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
        messageBuilder.AppendLine($"An unhandled .NET runtime exception ({ex.GetType().FullName}) occurred. {ex.Message}");

        messageBuilder.AppendLine();
        messageBuilder.AppendLine("This generally happens as a result of a broken mod or a bug in the game. Details have been logged to the game's log.");

        Log.Error(ex.ToString());

        dialog.Message = messageBuilder.ToString();
        dialog.MessageType = MessageBoxType.Error;
        dialog.Buttons.Add("OK");
        dialog.DismissCallback = completionSource.SetResult;

        return completionSource.Task;
    }

    private void OnGameModeChanged(GameMode gameMode)
    {
        trayModel.UpdateGameMode(gameMode);
        mainBox.Content = null;

        if (gameMode != GameMode.OnDesktop)
        {
            desktopController.InfoPanelController.ShowClock = false;
            desktopController.Logout();
            desktop?.Dispose();
        }

        switch (gameMode)
        {
            case GameMode.OnDesktop:
            {
                this.desktop = new Desktop(desktopController);
                this.mainBox.Content = desktop;

                desktopController.Login();
                desktopController.InfoPanelController.ShowClock = true;
                break;
            }
            case GameMode.AtLoginScreen:
            {
                var menu = new LoginScreen(context);
                mainBox.Content = menu;
                menu.Start();
                break;
            }
            case GameMode.CharacterCreator:
            {
                var menu = new CharacterCreatorMenu(context);
                mainBox.Content = menu;
                menu.Start();
                break;
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (desktopController.InfoPanelController.ShowClock)
        {
            desktopController.InfoPanelController.SetClock(context.WorldManager.World.GlobalWorldState.Value.Now);
        }
    }

    public bool OpenProgram(
        IProgram programToOpen,
        string[] arguments,
        ISystemProcess programProcess,
        ITextConsole console
    )
    {
        var window = this.desktopController.CreateFloatingApplicationWindow(programToOpen.Icon, programToOpen.WindowTitle);
        
        programToOpen.InstantiateIntoWindow(programProcess, window, console, arguments);

        return true;
    }

    public INotificationManager NotificationManager => notificationManager;
    public IInfoPanelService InfoPanelService => desktopController.InfoPanelController;

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
        this.overlays.ChildWidgets.Add(workspace);
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
            overlays.ChildWidgets.Remove(overlay);
        }
    }

    public Window CreateFloatingWindow(string title)
    {
        return this.floatingWindowArea.CreateWindow(title);
    }
    
    public void OpenSettings()
    {
        if (systemSettings != null)
            return;
        
        var overlay = CreateOverlayWorkspace();
        var window = overlay.CreateWindow("System Settings");

        window.Icon = MaterialIcons.Settings;

        systemSettings = new SystemSettingsController(window, this.context);

        window.SetClient(systemSettings);
        
        window.WindowClosed += HandleSettingsClosed;

        void HandleSettingsClosed(IWindow obj)
        {
            window.WindowClosed -= HandleSettingsClosed;

            systemSettings?.Dispose();
            systemSettings = null;

            overlays.ChildWidgets.Remove(overlay);
        }
    }
}