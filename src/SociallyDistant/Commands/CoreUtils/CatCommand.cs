#nullable enable
using SociallyDistant.Architecture;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Commands.CoreUtils
{
	[Command("cat")]
	public class CatCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			if (Arguments.Length == 0)
			{
				Console.WriteLine("cat: usage: cat <filepath>");
				return Task.CompletedTask;
			}

			string filePath = PathUtility.Combine(CurrentWorkingDirectory, Arguments[0]);

			if (!FileSystem.FileExists(filePath))
			{
				Console.WriteLine($"cat: {filePath}: File not found.");
				return Task.CompletedTask;
			}

			string fileText = FileSystem.ReadAllText(filePath);
			Console.WriteLine(fileText);
			
			return Task.CompletedTask;
		}

		public CatCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}