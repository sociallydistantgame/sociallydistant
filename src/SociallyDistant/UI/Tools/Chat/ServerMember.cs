using Microsoft.Xna.Framework.Graphics;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ServerMember
{
    public Texture2D? Avatar { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Handle { get; set; } = string.Empty;
}