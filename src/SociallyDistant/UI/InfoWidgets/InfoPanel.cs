using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Shell.InfoPanel;

namespace SociallyDistant.UI.InfoWidgets;

public sealed class InfoPanel : Widget
{
    private readonly FlexPanel                            root              = new();
    private readonly InfoPanelClock                       clock             = new();
    private readonly ObservableCollection<InfoWidgetData> observableWidgets = new();
    private readonly InfoWidgetAdapter                    widgetAdapter     = new();

    public event Action<int>? ItemClosed;
    
    public InfoPanel(ObservableCollection<InfoWidgetData> widgetsObservable)
    {
        this.observableWidgets = widgetsObservable;
        root.Padding = 10;
        root.Spacing = 10;
        root.Direction = Direction.Vertical;
        
        Children.Add(root);
        root.ChildWidgets.Add(clock);
        root.ChildWidgets.Add(widgetAdapter);
        
        this.observableWidgets.CollectionChanged += OnWidgetListChanged;
        widgetAdapter.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;

        this.widgetAdapter.MinimumSize = new Point(329, 0);
        this.widgetAdapter.MaximumSize = new Point(329, 0);
        
        widgetAdapter.ItemClosed += OnItemClosed;
    }

    private void OnItemClosed(int itemId)
    {
        ItemClosed?.Invoke(itemId);
    }

    private void OnWidgetListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        widgetAdapter.SetWidgets(observableWidgets);
    }
}