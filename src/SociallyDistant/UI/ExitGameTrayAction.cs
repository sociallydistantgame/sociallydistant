using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Shell;

namespace SociallyDistant.UI;

internal sealed class ExitGameTrayAction : TrayAction
{
    public ExitGameTrayAction(IGameContext context) : base(context)
    {
        Icon = MaterialIcons.PowerSettingsNew;
    }

    public override async void Invoke()
    {
        bool willExit = await AskToQuit();

        if (!willExit)
            return;

        await Context.EndCurrentGame(true);

        // bye bye
        Context.GameInstance.Exit();
    }
    
    private Task<bool> AskToQuit()
    {
        var completionSource = new TaskCompletionSource<bool>();
        var dialog = this.Context.Shell.CreateMessageDialog("Quit Socially Distant");

        dialog.Message = "Are you sure you want to quit the game? Any unsaved files will be lost.";
        dialog.MessageType = MessageBoxType.Warning;
        
        dialog.Buttons.Add(new MessageBoxButtonData("Yes", MessageDialogResult.Yes));
        dialog.Buttons.Add(new MessageBoxButtonData("No",  MessageDialogResult.No));

        dialog.DismissCallback = result =>
        {
            completionSource.SetResult(result == MessageDialogResult.Yes);
        };

        return completionSource.Task;
    }
}