using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Social;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class TypingIndicator : Widget
{
    private readonly StackPanel                root    = new();
    private readonly TextWidget                label   = new();
    private readonly TypingIndicatoravatarList avatars = new();

    public TypingIndicator()
    {
        root.Direction = Direction.Horizontal;
        root.Spacing = 6;
        label.Text = "typing...";
        
        Children.Add(root);
        root.ChildWidgets.Add(avatars);
        root.ChildWidgets.Add(label);
    }

    public void UpdateIndicator(IEnumerable<IProfile> typers)
    {
        var typersList = typers as IProfile[] ?? typers.ToArray();
        if (!typersList.Any())
        {
            Visibility = Visibility.Hidden;
            return;
        }

        Visibility = Visibility.Visible;
        avatars.SetItems(typersList.Select(x=>x.Picture));
    }
}