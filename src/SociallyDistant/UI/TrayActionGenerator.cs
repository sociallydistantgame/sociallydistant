using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.UI;

internal sealed class TrayActionGenerator : IContentGenerator
{
    private readonly IGameContext context;

    public TrayActionGenerator(IGameContext context)
    {
        this.context = context;
    }
    
    public IEnumerable<IGameContent> CreateContent()
    {
        yield return new SystemSettingsTrayAction(context);
        yield return new ExitSessionTrayAction(context);
        yield return new ExitGameTrayAction(context);
    }
}