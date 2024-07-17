using AcidicGUI.Widgets;
using SociallyDistant.Core.Social;

namespace SociallyDistant.UI.WebSites.SocialMedia;

public sealed class Timeline : Widget
{
    private readonly SocialPostAdapter adapter = new();

    public Timeline()
    {
        Children.Add(adapter);
    }

    public void ShowPosts(IEnumerable<IUserMessage> posts)
    {
        adapter.SetItems(posts);
    }
}