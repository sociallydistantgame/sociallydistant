using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.GamePlatform.ContentManagement;

internal class HookScriptSource : IGameContentSource
{
    public async Task LoadAllContent(ContentCollectionBuilder builder, IContentFinder finder)
    {
        foreach (HookScript hookScript in await finder.FindContentOfType<HookScript>())
        {
            builder.AddContent(hookScript);
        }
    }
}