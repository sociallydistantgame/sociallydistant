using SociallyDistant.Architecture;
using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant;

public sealed class LifepathGenerator : IContentGenerator
{
    public IEnumerable<IGameContent> CreateContent()
    {
        yield return new LifepathAsset("redshedder", "Redshedder", "Placeholder text");
        yield return new LifepathAsset("cybercriminal", "Cybercriminal", "Placeholder text");
        
    }
}