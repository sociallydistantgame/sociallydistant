using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.Common;

public sealed class StatusBarUserDisplay : Widget
{
    private readonly StackPanel stack  = new();
    private readonly TextWidget text   = new();
    private readonly Avatar     avatar = new();

    private IUser? user;
    
    public IUser? User
    {
        get => user;
        set
        {
            user = value;
            UpdateWidgets();
        }
    }
    
    public StatusBarUserDisplay()
    {
        text.UseMarkup = false;
        text.WordWrapping = false;
        text.Font = PresetFontFamily.Monospace;
        text.VerticalAlignment = VerticalAlignment.Middle;

        avatar.AvatarSize = 24;
        avatar.VerticalAlignment = VerticalAlignment.Middle;
        
        stack.Direction = Direction.Horizontal;
        stack.Spacing = 3;
        
        Children.Add(stack);
        stack.ChildWidgets.Add(avatar);
        stack.ChildWidgets.Add(text);
    }

    private void UpdateWidgets()
    {
        if (user == null)
        {
            this.Visibility = Visibility.Collapsed;
            text.Text = string.Empty;
        }
        else
        {
            this.Visibility = Visibility.Visible;
            text.Text = $"{user.UserName}@{user.Computer.Name}";
        }
    }
}