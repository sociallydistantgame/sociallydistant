using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Shell.InfoPanel;

namespace SociallyDistant.UI.InfoWidgets;

public sealed class InfoPanelClock : Widget
{
    private readonly StackPanel root      = new();
    private readonly TextWidget timeOfDay = new();
    private readonly TextWidget dayOfWeek = new();
    private          int        hour;
    private          int        minute;
    private          int        day;
    
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

    public void SetClock(DateTime date)
    {
        if (hour == date.Hour && minute == date.Minute && day == date.Day)
            return;

        timeOfDay.Text = date.ToShortTimeString();
        dayOfWeek.Text = SociallyDistantUtility.GetDayOfWeek(date.DayOfWeek);

        hour = date.Hour;
        minute = date.Minute;
        day = date.Day;
    }
}