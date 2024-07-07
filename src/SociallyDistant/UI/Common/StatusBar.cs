using System.Diagnostics.Contracts;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.Common;

public class StatusBar : Widget
{
    private readonly FlexPanel            flexPanel   = new();
    private readonly StatusBarUserDisplay userDisplay = new();
    private readonly TrayModel            trayModel;
    private readonly SystemTray           tray;

    public IUser? User
    {
        get => userDisplay.User;
        set => userDisplay.User = value;
    }
    
    internal StatusBar(TrayModel trayModel)
    {
        this.trayModel = trayModel;
        
        tray = new SystemTray(trayModel);
        
        MinimumSize = new Vector2(0, 24);
        Margin = 3;

        flexPanel.Direction = Direction.Horizontal;
        flexPanel.Spacing = 3;

        Children.Add(flexPanel);

        flexPanel.ChildWidgets.Add(userDisplay);
        flexPanel.ChildWidgets.Add(tray);
        
        SetCustomProperty(WidgetBackgrounds.StatusBar);
    }
}