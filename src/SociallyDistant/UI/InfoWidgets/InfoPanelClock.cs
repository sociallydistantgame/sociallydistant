using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.InfoPanel;

namespace SociallyDistant.UI.InfoWidgets;

public sealed class InfoPanelClock : Widget
{
    private readonly StackPanel                           root              = new();
    private readonly TextWidget                           timeOfDay         = new();
    private readonly TextWidget                           dayOfWeek         = new();
    
    public InfoPanelClock()
    {
        timeOfDay.TextAlignment = TextAlignment.Center;
        dayOfWeek.TextAlignment = TextAlignment.Center;
        timeOfDay.FontSize = 36;
        timeOfDay.FontWeight = FontWeight.Bold;

        root.Direction = Direction.Vertical;
        
        Children.Add(root);
        root.ChildWidgets.Add(timeOfDay);
        root.ChildWidgets.Add(dayOfWeek);

        timeOfDay.Text = "12:42 PM";
        dayOfWeek.Text = "Wednesday";
    }
}