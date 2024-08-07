using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.UI.WebSites.SocialMedia;

public class SocialPostModel
{
    public Texture2D? Avatar { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Handle { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public int RepostCount { get; set; }
    public int ReplyCount { get; set; }
    public int StarCount { get; set; }
    public DocumentElement[] Document { get; set; } = Array.Empty<DocumentElement>();
}