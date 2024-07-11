#nullable enable

using SociallyDistant.Architecture;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Commands.CoreUtils
{
	[Command("ls")]
	public class ListDirectoryCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			// TODO: Listing directories specified in arguments
			// TODO: Colorful output
			// TODO: Different output styles
			// error out if the current directory doesn't exist
			if (!FileSystem.DirectoryExists(CurrentWorkingDirectory))
			{
				Console.WriteLine($"ls: {CurrentWorkingDirectory}: Directory not found.");
				return Task.CompletedTask;
			}

			foreach (string directory in FileSystem.GetDirectories(CurrentWorkingDirectory))
			{
				string filename = PathUtility.GetFileName(directory);
				Console.WriteLine(filename);
			}
			
			foreach (string directory in FileSystem.GetFiles(CurrentWorkingDirectory))
			{
				string filename = PathUtility.GetFileName(directory);
				Console.WriteLine(filename);
			}
			
			return Task.CompletedTask;
		}

		public ListDirectoryCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}