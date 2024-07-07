using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.UI.Shell;

namespace SociallyDistant.UI.Common;

public sealed class SystemTray : Widget
{
    private readonly TrayModel        trayModel;
    private readonly List<TrayAction> actions    = new();
    private readonly StackPanel       stackPanel = new();
    private readonly IDisposable      actionsObserver;
    private readonly List<TrayIcon>   icons = new();

    internal SystemTray(TrayModel trayModel)
    {
        this.trayModel = trayModel;
        
        Children.Add(stackPanel);

        stackPanel.Spacing = 3;
        stackPanel.Direction = Direction.Horizontal;

        actionsObserver = trayModel.TrayActions.Subscribe(RefreshActions);
    }

    private void RefreshActions(IEnumerable<TrayAction> trayActions)
    {
        this.actions.Clear();
        this.actions.AddRange(trayActions);

        this.UpdateViews();
    }

    private void UpdateViews()
    {
        while (icons.Count > actions.Count)
        {
            stackPanel.ChildWidgets.Remove(icons[^1]);
            icons.RemoveAt(icons.Count-1);
        }

        for (var i = 0; i < actions.Count; i++)
        {
            if (i == icons.Count)
            {
                var icon = new TrayIcon();
                stackPanel.ChildWidgets.Add(icon);
                icons.Add(icon);
                icon.Action = actions[i];
            }
            else
            {
                icons[i].Action = actions[i];
            }
        }
    }
}