using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Social;

namespace SociallyDistant.UI.Tools.Chat;

public class GuildItemModel
{
    public Texture2D? GuildIcon { get; set; }
    public IGuild? Guild { get; set; }
    public bool IsSelected { get; set; }
}