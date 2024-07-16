using AcidicGUI.Layout;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework.Graphics;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class TypingIndicatoravatarList : ListAdapter<StackPanel, TypingIndicatorAvatarViewHolder>
{
    private readonly DataHelper<Texture2D?> textures;

    public TypingIndicatoravatarList()
    {
        textures = new DataHelper<Texture2D?>(this);

        Container.Direction = Direction.Horizontal;
        Container.Spacing = -8; // smushes the avatar together like a Twitter group DM And no. Don't submit a PR changing "Twitter" to "X", I'll ban you.
    }

    public void SetItems(IEnumerable<Texture2D?> source)
    {
        textures.SetItems(source);
    }
    
    protected override TypingIndicatorAvatarViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new TypingIndicatorAvatarViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(TypingIndicatorAvatarViewHolder viewHolder)
    {
        viewHolder.AvatarTexture = textures[viewHolder.ItemIndex];
    }
}