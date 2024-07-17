using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Social;

namespace SociallyDistant.UI.WebSites.SocialMedia;

public sealed class SocialPostAdapter : ListAdapter<ScrollView, SocialPostViewHolder>
{
    private readonly DataHelper<SocialPostModel> items;

    public SocialPostAdapter()
    {
        items = new DataHelper<SocialPostModel>(this);
    }

    public void SetItems(IEnumerable<IUserMessage> posts)
    {
        items.SetItems(posts.Select(ConvertToModel));
    }
    
    protected override SocialPostViewHolder CreateViewHolder(int itemIndex, Box rootWidget)
    {
        return new SocialPostViewHolder(itemIndex, rootWidget);
    }

    protected override void UpdateView(SocialPostViewHolder viewHolder)
    {
        viewHolder.UpdateView(items[viewHolder.ItemIndex]);
    }
    
    private SocialPostModel ConvertToModel(IUserMessage message)
    {
        return new SocialPostModel
        {
            Document = message.GetDocumentData(),
            Name = message.Author.ChatName,
            Handle = message.Author.SocialHandle
        };
    }
}