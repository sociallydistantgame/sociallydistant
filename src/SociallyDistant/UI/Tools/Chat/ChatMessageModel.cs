using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ChatMessageModel
{
    public ObjectId        Id;
    public DateTime        Date;
    public ObjectId        AuthorId;
    public Texture2D?      Avatar;
    public bool            UseBubbleStyle;
    public bool            IsFromPlayer;
    public string          DisplayName       = string.Empty;
    public string          Username          = string.Empty;
    public string          FormattedDateTime = string.Empty;
    public DocumentElement Document;
    public bool            ShowAvatar;
    public bool            IsNewMessage;
}