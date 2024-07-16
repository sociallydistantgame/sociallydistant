using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class TypingIndicatorAvatarViewHolder : ViewHolder
{
    private readonly Avatar view = new();

    public Texture2D? AvatarTexture
    {
        get => view.AvatarTexture;
        set => view.AvatarTexture = value;
    }
    
    public TypingIndicatorAvatarViewHolder(int itemIndex, Box root) : base(itemIndex, root)
    {
        root.Content = view;
        view.AvatarSize = 16;
    }
}