using AcidicGUI.CustomProperties;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;

namespace SociallyDistant.Core.UI.Effects;

public sealed class DefaultAvatarProperties : CustomPropertyObject
{
    private bool  useLightBackground = false;
    private Color foreground         = Color.Cyan;

    public bool UseLightBackground
    {
        get => useLightBackground;
        set => useLightBackground = value;
    }

    public Color Foreground
    {
        get => foreground;
        set
        {
            foreground = value;
            Widget.InvalidateGeometry();
        }
    } 
    
    public DefaultAvatarProperties(Widget owner) : base(owner)
    {
    }
}