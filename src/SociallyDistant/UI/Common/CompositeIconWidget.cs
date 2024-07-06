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

    private void UpdateIconSize()
    {
        image.MaximumSize = new Vector2(iconSize, iconSize);
        image.MinimumSize = image.MaximumSize;
        textIcon.IconSize = iconSize;
    }

    public CompositeIconWidget()
    {
        Children.Add(image);
        Children.Add(textIcon);
    }
    
    private void UpdateIcon()
    {
        image.Texture = compositeIcon.spriteIcon;
        textIcon.IconString = compositeIcon.textIcon;

        textIcon.Color = compositeIcon.iconColor;
    }
}