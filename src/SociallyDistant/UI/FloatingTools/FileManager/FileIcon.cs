using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.UI.Common;

namespace SociallyDistant.UI.FloatingTools.FileManager;

public sealed class FileIcon : Widget,
    IMouseClickHandler
{
    private readonly StackPanel          root = new();
    private readonly CompositeIconWidget icon = new();
    private readonly TextWidget          text = new();
    private          float               clickTime;
    private          Action<int>?        clickHandler;
    private readonly float               doubleClickTime = 0.3f;
    private          int                 id;

    public FileIcon()
    {
        root.Spacing = 3;
        icon.IconSize = 40;
        icon.HorizontalAlignment = HorizontalAlignment.Center;
        
        text.TextAlignment = TextAlignment.Center;
        text.FontWeight = FontWeight.Medium;
        
        Children.Add(root);
        root.ChildWidgets.Add(icon);
        root.ChildWidgets.Add(text);

        MinimumSize = new Point(75, 0);
        MaximumSize = new Point(75, 0);
        
    }
    
    public void UpdateView(FileIconModel model)
    {
        id = model.Id;
        this.icon.Icon = model.Icon;
        this.text.Text = model.Title;
        this.clickHandler = model.OpenHandler;
    }

    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
        {
            clickTime = 0;
            return;
        }

        e.RequestFocus();

        float currentTime = Time.time;
        if (currentTime - clickTime <= doubleClickTime)
        {
            clickHandler?.Invoke(id);
        }

        clickTime = currentTime;
    }
}