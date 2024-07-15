using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant;

internal sealed class CommandAsset : 
    INamedAsset,
    ICommandTask
{
    private readonly IGameContext                     context;
    private readonly CommandAttribute                 attribute;
    private readonly Func<IGameContext, ICommandTask> constructor;

    public string Name => attribute.Name;

    public CommandAsset(IGameContext context, CommandAttribute attribute, Func<IGameContext, ICommandTask> constructor)
    {
        this.context = context;
        this.attribute = attribute;
        this.constructor = constructor;
    }

    public Task Main(ISystemProcess process, ITextConsole console, string[] arguments)
    {
        var task = constructor(context);
        return task.Main(process, console, arguments);
    }
}