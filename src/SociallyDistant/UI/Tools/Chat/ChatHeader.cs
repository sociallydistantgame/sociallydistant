using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.Core.UI.VisualStyles;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ChatHeader : Widget
{
    private readonly StackPanel          root     = new();
    private readonly CompositeIconWidget icon     = new();
    private readonly StackPanel          textArea = new();
    private readonly TextWidget          title    = new();
    private readonly TextWidget          subtitle = new();

    public ChatHeader()
    {
        root.Direction = Direction.Horizontal;
        root.Spacing = 6;

        icon.VerticalAlignment = VerticalAlignment.Middle;
        textArea.VerticalAlignment = VerticalAlignment.Middle;

        icon.IconSize = 48;
        icon.Icon = MaterialIcons.Comment;
        title.FontWeight = FontWeight.SemiBold;
        title.FontSize = 24;
        title.SetCustomProperty(WidgetForegrounds.Common);
        
        Children.Add(root);
        root.ChildWidgets.Add(icon);
        root.ChildWidgets.Add(textArea);
        textArea.ChildWidgets.Add(title);
        textArea.ChildWidgets.Add(subtitle);
    }

    public void DisplayGuildHeader(string name, string description)
    {
        this.icon.Icon = MaterialIcons.ChatBubble;
    }

    public void DisplayDirectMessage(string name, string description, Texture2D? avatar)
    {
        icon.Icon = MaterialIcons.AccountCircle;
        this.title.Text = name;
        this.subtitle.Text = description;
    }
}