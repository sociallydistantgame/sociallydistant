using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.UI.Common;

public class CompositeIconWidget : Widget
{
    private readonly Image image = new();
    private readonly Icon textIcon = new();
    
    private CompositeIcon compositeIcon;
    private int iconSize = 24;

    public int IconSize
    {
        get => iconSize;
        set
        {
            iconSize = value;
            UpdateIconSize();
        }
    }

    public CompositeIcon Icon
    {
        get => compositeIcon;
        set
        {
            compositeIcon = value;
            UpdateIcon();
        }
    }

    public CompositeIconWidget()
    {
        Children.Add(image);
        Children.Add(textIcon);
        
        UpdateIconSize();
        UpdateIcon();
    }
    
    private void UpdateIconSize()
    {
        image.MaximumSize = new Point(iconSize, iconSize);
        image.MinimumSize = image.MaximumSize;
        textIcon.IconSize = iconSize;
    }
    
    private void UpdateIcon()
    {
        image.Texture = compositeIcon.spriteIcon;
        textIcon.IconString = compositeIcon.textIcon;

        textIcon.Color = compositeIcon.iconColor;
        
        image.Visibility = image.Texture != null
            ? Visibility.Visible
            : Visibility.Collapsed;
        
        textIcon.Visibility = image.Texture == null
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}