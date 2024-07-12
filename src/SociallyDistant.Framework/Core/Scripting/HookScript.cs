using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.Scripting.Instructions;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;

public sealed class HookScript : ShellScriptAsset,
    IHookListener,
    IGameContent
{
    private readonly string hookId;
    private readonly string scriptText;

    private ShellInstruction? program;

    public string HookId => hookId;
    
    public HookScript(string hookId, string scriptText)
    {
        this.hookId = hookId;
        this.scriptText = scriptText;
    }


    private async Task RebuildScriptTree(IGameContext game)
    {
        var context = new UserScriptExecutionContext();
        var shell = new InteractiveShell(context);

        program = await shell.ParseScript(scriptText);
    }
    
    public async Task ReceiveHookAsync(IGameContext game)
    {
        if (program == null)
            await RebuildScriptTree(game);

        var context = new UserScriptExecutionContext();
        context.ModuleManager.RegisterModule(new WorldScriptCommands(game));

        var shell = new InteractiveShell(context);
        shell.Setup(new NullConsole());

        await shell.RunParsedScript(program!);
    }
}