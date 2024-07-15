using System.Reflection;
using Serilog;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant;

public sealed class CommandFinder : IContentGenerator
{
    private readonly IGameContext context;

    public CommandFinder(IGameContext context)
    {
        this.context = context;
    }

    public IEnumerable<IGameContent> CreateContent()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsAssignableTo(typeof(ICommandTask)))
                    continue;

                var constructor = type.GetConstructor(new[] { typeof(IGameContext) });
                if (constructor == null)
                    continue;

                var attribute = type.GetCustomAttributes(false).OfType<CommandAttribute>().FirstOrDefault();

                if (attribute == null)
                    continue;

                yield return new CommandAsset(context, attribute, (ctx) =>
                {
                    try
                    {
                        return (ICommandTask)constructor.Invoke(new[] { ctx });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                        throw;
                    }
                });
            }
        }
    }
}