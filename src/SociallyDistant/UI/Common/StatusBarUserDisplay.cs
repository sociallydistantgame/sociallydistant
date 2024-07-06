using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Shell;

namespace SociallyDistant.UI.Common;

public sealed class StatusBarUserDisplay : Widget
{
    private readonly StackPanel stack = new();
    private readonly TextWidget text = new();
    
    // TODO: Port the Avatar widget
    private readonly Icon avatar = new();

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
        text.Font = FontPreset.Monospace;

        avatar.IconSize = 21;
        avatar.IconString = MaterialIcons.AccountCircle;
        
        stack.Direction = Direction.Horizontal;
        stack.Spacing = 3;
        
        Children.Add(stack);
        stack.ChildWidgets.Add(avatar);
        stack.ChildWidgets.Add(text);
    }

    private void UpdateWidgets()
    {
        if (user == null)
            text.Text = string.Empty;
        else
            text.Text = $"{user.UserName}@{user.Computer.Name}";
    }
}