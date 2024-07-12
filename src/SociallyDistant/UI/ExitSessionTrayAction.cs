using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Shell;

namespace SociallyDistant.UI;

internal sealed class ExitSessionTrayAction : TrayAction
{
    public ExitSessionTrayAction(IGameContext context) : base(context)
    {
        this.Icon = MaterialIcons.Logout;
    }

    public override bool CanUseInGameMode(GameMode gameMode)
    {
        return gameMode == GameMode.OnDesktop;
    }

    public override async void Invoke()
    {
        var willExit = await AskToQuit();

        if (!willExit)
            return;

        await Context.GoToLoginScreen();
    }

    private Task<bool> AskToQuit()
    {
        var completionSource = new TaskCompletionSource<bool>();
        var dialog = this.Context.Shell.CreateMessageDialog("End session");

        dialog.Message = "Are you sure you want to end the session and log out? Any unsaved files will be lost.";
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