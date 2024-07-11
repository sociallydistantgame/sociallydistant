#nullable enable
using System.Collections;
using SociallyDistant.Architecture;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Commands.CoreUtils
{
	[Command("sleep")]
	public class SleepCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override async Task OnExecute()
		{
			if (Arguments.Length < 1)
			{
				Console.WriteLine("sleep: usage: sleep <milliseconds>");
				return;
			}

			if (!int.TryParse(Arguments[0], out int milliseconds))
			{
				Console.WriteLine("sleep: Unexpected time value " + Arguments[0]);
				return;
			}

			await Task.Delay(milliseconds);
		}

		public SleepCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}