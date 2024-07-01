using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.GamePlatform.ContentManagement;

internal sealed class XnaContentFinder : IContentFinder
{
    private readonly ContentPipeline pipeline;

    public XnaContentFinder(ContentPipeline pipeline)
    {
        this.pipeline = pipeline;
    }
		
    public async Task<T[]> FindContentOfType<T>()
    {
        var assets = new List<T>();
        foreach (T asset in pipeline.LoadAll<T>())
        {
            assets.Add(asset);
            await Task.Yield();
        }

        return assets.ToArray();
    }
}