using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.UI.Windowing;

namespace SociallyDistant.UI;

public sealed class FloatingToolLauncher
{
    private readonly DesktopController        desktopController;
    private readonly GuiController            gui;
    private readonly DockGroup                dockGroup;
    private readonly DockGroup                taskManagerGroup;
    private readonly DockGroup.IconDefinition openProgramIcon;
    private readonly List<IWindow>            appWindows = new();
    private          Window?                  launcherWindow;

    internal FloatingToolLauncher(DesktopController desktopController, GuiController gui)
    {
        this.desktopController = desktopController;
        this.gui = gui;
        dockGroup = desktopController.DockModel.DefineGroup();
        dockGroup.Slot = DockSlot.Bottom;

        taskManagerGroup = desktopController.DockModel.DefineGroup();
        taskManagerGroup.Slot = DockSlot.Middle;

        this.openProgramIcon = new DockGroup.IconDefinition() { Icon = MaterialIcons.Apps, Label = "Programs", ClickHandler = ToggleLauncher };
        dockGroup.Add(openProgramIcon);
    }

    public IContentPanel CreateWindow(CompositeIcon icon, string title)
    {
        var win = gui.CreateFloatingWindow(title);
        win.Icon = icon;
        this.appWindows.Add(win);
        
        win.WindowClosed += HandleWindowClosed;
        RefreshDock();
        return win.ContentPanel;
    }

    private void HandleWindowClosed(IWindow win)
    {
        win.WindowClosed -= HandleWindowClosed;
        
        appWindows.Remove(win);
        RefreshDock();
    }

    private void RefreshAppDock()
    {
        while (taskManagerGroup.Count > appWindows.Count)
            taskManagerGroup.RemoveAt(taskManagerGroup.Count -1);

        for (var i = 0; i < appWindows.Count; i++)
        {
            if (i == taskManagerGroup.Count)
            {
                taskManagerGroup.Add(new DockGroup.IconDefinition());
            }

            var id = i;
            var icon = taskManagerGroup[i];
            icon.Icon = appWindows[i].Icon;
            icon.ClickHandler = () => OnTaskManagerIconClicked(id);
        }

        taskManagerGroup.RefreshDock();
    }

    private void OnTaskManagerIconClicked(int index)
    {
        throw new NotImplementedException();
    }

    private void RefreshDock()
    {
        RefreshAppDock();
        
        this.openProgramIcon.IsActive = launcherWindow != null;
        dockGroup.RefreshDock();
    }
    
    private void ToggleLauncher()
    {
        if (launcherWindow != null)
        {
            launcherWindow.WindowClosed -= OnLauncherClosed;
            launcherWindow.ForceClose();
            launcherWindow = null;
        }
        else
        {
            launcherWindow = gui.CreateFloatingWindow("Programs");
            launcherWindow.WindowClosed += OnLauncherClosed;

            this.launcherWindow.ContentPanel.Content = new AppLauncher(desktopController, gui, this.launcherWindow);
        }

        RefreshDock();
    }

    private void OnLauncherClosed(IWindow obj)
    {
        launcherWindow = null;
        RefreshDock();
    }
}