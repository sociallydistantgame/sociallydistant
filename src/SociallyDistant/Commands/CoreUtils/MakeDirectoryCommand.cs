#nullable enable
using SociallyDistant.Architecture;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Commands.CoreUtils
{
	[Command("mkdir")]
	public class MakeDirectoryCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			if (Arguments.Length == 0)
			{
				Console.WriteLine("mkdir: usage: cat <filepath>");
				return Task.CompletedTask;
			}

			string fullPath = PathUtility.Combine(CurrentWorkingDirectory, Arguments[0]);
			
			if (!FileSystem.DirectoryExists(fullPath))
				FileSystem.CreateDirectory(fullPath);
			
			return Task.CompletedTask;
		}

		public MakeDirectoryCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}