using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.Shell;

public sealed class Dock : Widget, 
    IDisposable
{
    private readonly DesktopController desktopController;
    private readonly Box               root      = new();
    private readonly FlexPanel         dockItems = new();
    private readonly IDisposable       dockObserver;
    private readonly DockGroupAdapter  topGroup    = new();
    private readonly DockGroupAdapter  middleGroup = new();
    private readonly DockGroupAdapter  bottomGroup = new();

    internal Dock(DesktopController desktopController)
    {
        this.desktopController = desktopController;
        
        MinimumSize = new Point(27, 0);
        
        root.Margin = 3;
        dockItems.Spacing = 3;
        
        Children.Add(root);
        root.Content = dockItems;

        root.SetCustomProperty(WidgetBackgrounds.Dock);

        dockObserver = desktopController.DockModel.DockGroupsObservable.Subscribe(OnDockRefreshed);

        dockItems.ChildWidgets.Add(topGroup);
        dockItems.ChildWidgets.Add(middleGroup);
        dockItems.ChildWidgets.Add(bottomGroup);
        
        topGroup.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        middleGroup.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        bottomGroup.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;

        topGroup.VerticalAlignment = VerticalAlignment.Top;
        middleGroup.VerticalAlignment = VerticalAlignment.Middle;
        bottomGroup.VerticalAlignment = VerticalAlignment.Bottom;
    }

    public void Dispose()
    {
        dockObserver?.Dispose();
    }

    private void OnDockRefreshed(IEnumerable<DockGroup> groups)
    {
        var allGroups = groups.ToArray();

        var top = new List<DockGroup.IconDefinition>();
        var middle = new List<DockGroup.IconDefinition>();
        var bottom = new List<DockGroup.IconDefinition>();

        foreach (DockGroup group in allGroups)
        {
            foreach (DockGroup.IconDefinition icon in group)
            {
                switch (group.Slot)
                {
                    case DockSlot.Top:
                        top.Add(icon);
                        break;
                    case DockSlot.Middle:
                        middle.Add(icon);
                        break;
                    case DockSlot.Bottom:
                        bottom.Add(icon);
                        break;
                }
            }
        }

        Visibility = Visibility.Collapsed;

        topGroup.SetItems(top);
        middleGroup.SetItems(middle);
        bottomGroup.SetItems(bottom);
        
        Visibility = Visibility.Visible;
    }
}