#nullable enable
using System.Text;
using SociallyDistant.Architecture;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Commands.Misc
{
	[Command("cowsay")]
	public class CowsayCommand : ScriptableCommand
	{
		private readonly string cow = @"        \   ^__^
         \  (oo)\_______
            (__)\       )\/\
                ||----w |
                ||     ||
";
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			string text = string.Empty;
			if (Arguments.Length > 0)
			{
				text = string.Join(" ", Arguments);
			}
			else
			{
				var sb = new StringBuilder();
				while (Console.ReadLine(out string line))
					sb.AppendLine(line);
				text = sb.ToString();
			}

			string speechBubble = MakeSpeechBubble(text, 40);
			
			Console.WriteLine(speechBubble.Trim());
			Console.WriteLine(cow);
			return Task.CompletedTask;
		}


		private string WordWrap(string line, int lineWrap)
		{
			var wrapBuilder = new StringBuilder();
			var lineCharCount = 0;

			for (var i = 0; i < line.Length; i++)
			{
				char character = line[i];

				if (lineCharCount == lineWrap)
				{
					lineCharCount = 0;
					wrapBuilder.AppendLine();
				}

				wrapBuilder.Append(character);
				lineCharCount++;
			}
			
			return wrapBuilder.ToString();
		}

		private string MakeSpeechBubble(string text, int textWrap)
		{
			var sb = new StringBuilder();
			
			string[] lines = text.Split(Environment.NewLine);
			foreach (string line in lines)
			{
				sb.AppendLine(WordWrap(line, textWrap));
			}

			// Split the wrapped lines so we can build the speech bubble.
			lines = sb.ToString().TrimEnd().Split(Environment.NewLine);
			sb.Length = 0;

			var padding = 1;
			int longestLine = lines.Length > 0
				? lines.Select(x => x.Length)
					.OrderByDescending(x => x)
					.First()
				: 1;
			
			// Top dashes
			sb.Append(" ");

			for (var i = 0; i < longestLine + (padding * 2); i++)
				sb.Append("_");
			sb.AppendLine();

			if (lines.Length == 0)
			{
				sb.Append("<");

				for (var i = 0; i < longestLine + (padding * 2); i++)
					sb.Append(" ");
				
				sb.AppendLine(">");
			}
			else
			{
				for (var i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					int lineLength = line.Length;
					int spacing = longestLine - lineLength;

					char left = i switch
					{
						{ } when lines.Length == 1 => '<',
						{ } when i == lines.Length - 1 => '\\',
						0 => '/',
						_ => '|'
					};

					char right = i switch
					{
						{ } when lines.Length == 1 => '>',
						{ } when i == lines.Length - 1 => '/',
						0 => '\\',
						_ => '|'
					};

					sb.Append(left);
					for (var j = 0; j < padding; j++)
						sb.Append(" ");

					sb.Append(line);
					for (var j = 0; j < spacing; j++)
						sb.Append(" ");
					
					for (var j = 0; j < padding; j++)
						sb.Append(" ");

					sb.AppendLine(right.ToString());
				}
			}	
			
			// Bottom dashes
			sb.Append(" ");

			for (var i = 0; i < longestLine + (padding * 2); i++)
				sb.Append("-");
			sb.AppendLine();

			return sb.ToString();


		}

		public CowsayCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}