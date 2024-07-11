#nullable enable
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Architecture
{
	[Command("man")]
	public class ManualCommand : ScriptableCommand
	{
		
		private string manualUrl = "https://man.sociallydistantgame.com/";
		
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			System.Diagnostics.Process.Start(manualUrl);
			return Task.CompletedTask;
		}

		public ManualCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}