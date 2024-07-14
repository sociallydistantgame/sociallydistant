using AcidicGUI.Widgets;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Player;

namespace SociallyDistant.UI;

internal sealed class DesktopController
{
    private readonly PlayerManager playerManager;
    private readonly GuiController guiController;
    private readonly ToolManager   toolManager;
    private readonly DockModel     dockModel = new();
    
    private IUser? loginUser;
    private ISystemProcess loginProcess;

    public Widget ToolsRootWidget => toolManager.RootWidget;

    public DockModel DockModel => dockModel;
    
    public DesktopController(GuiController gui, PlayerManager player)
    {
        this.guiController = gui;
        this.playerManager = player;

        this.toolManager = new ToolManager(this, dockModel.DefineGroup());
    }

    public ISystemProcess Fork()
    {
        return loginProcess.Fork();
    }

    public void Logout()
    {
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
    }
}