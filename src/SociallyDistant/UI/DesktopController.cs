using AcidicGUI.Widgets;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Player;
using SociallyDistant.UI.Common;
using SociallyDistant.UI.InfoWidgets;
using SociallyDistant.UI.Shell;

namespace SociallyDistant.UI;

public sealed class BrowserSchemeHandler : IUriSchemeHandler
{
    private readonly ToolManager shell;

    internal BrowserSchemeHandler(ToolManager shell)
    {
        this.shell = shell;
    }
		
    /// <inheritdoc />
    public async void HandleUri(Uri uri)
    {
        // switch to (or open) the web browser in the Main Tile
        await shell.OpenWebBrowser(uri);
    }
}

internal sealed class DesktopController
{
    private readonly PlayerManager        playerManager;
    private readonly GuiController        guiController;
    private readonly ToolManager          toolManager;
    private readonly DockModel            dockModel           = new();
    private readonly InfoPanelController  infoPanelController = new();
    private readonly FloatingToolLauncher floatingToolLauncher;
    private readonly BrowserSchemeHandler browserHandler;
    
    private IUser? loginUser;
    private ISystemProcess loginProcess;

    public Widget ToolsRootWidget => toolManager.RootWidget;

    public DockModel DockModel => dockModel;

    public InfoPanelController InfoPanelController => infoPanelController;
    
    public DesktopController(GuiController gui, PlayerManager player)
    {
        this.guiController = gui;
        this.playerManager = player;

        this.toolManager = new ToolManager(this, dockModel.DefineGroup());

        floatingToolLauncher = new FloatingToolLauncher(this, gui);
        browserHandler = new BrowserSchemeHandler(this.toolManager);
    }

    public IContentPanel CreateFloatingApplicationWindow(CompositeIcon icon, string title)
    {
        return this.floatingToolLauncher.CreateWindow(icon, title);
    }
    
    public ISystemProcess Fork()
    {
        return loginProcess.Fork();
    }

    public void Logout()
    {
        guiController.Context.UriManager.UnregisterSchema("web");
        loginProcess?.Kill();
        loginUser = null;

        guiController.StatusBar.User = null;
        
        
    }
    
    public void Login()
    {
        loginUser = playerManager.PlayerUser;
        loginProcess = playerManager.InitProcess.CreateLoginProcess(loginUser);

        guiController.StatusBar.User = loginUser;

        toolManager.StartFirstTool();

        guiController.Context.UriManager.RegisterSchema("web", browserHandler);
    }
}