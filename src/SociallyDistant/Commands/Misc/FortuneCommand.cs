#nullable enable
using System.Text;
using SociallyDistant.Architecture;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Commands.Misc
{
	[Command("fortune")]
	public class FortuneCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override async Task OnExecute()
		{
			var fortuneList = new List<string>();
			using var stream = Game.GameInstance.Content.Load<Stream>("/Core/fortunes.txt");
			var builder = new StringBuilder();
			using (var reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
				{
					string line = await reader.ReadLineAsync() ?? string.Empty;

					if (line.StartsWith("#"))
						continue;

					if (line == "%" || reader.EndOfStream)
					{
						string fortune = builder.ToString().Trim();
						if (!string.IsNullOrWhiteSpace(fortune))
							fortuneList.Add(fortune);

						builder.Length = 0;
						continue;
					}

					builder.AppendLine(line);
				}
			}

			Console.WriteLine(fortuneList[new Random().Next(0, fortuneList.Count)]);
		}

		public FortuneCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}