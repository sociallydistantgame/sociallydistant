#nullable enable
namespace SociallyDistant.Architecture
{
	public class ManualCommand : ScriptableCommand
	{
		
		private string manualUrl = "https://man.sociallydistantgame.com/";
		
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			System.Diagnostics.Process.Start(manualUrl);
			return Task.CompletedTask;
		}
	}
}