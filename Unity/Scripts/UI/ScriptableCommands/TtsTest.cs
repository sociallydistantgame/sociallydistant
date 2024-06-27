#nullable enable
using System.Text;
using Accessibility;
using Architecture;
using UnityEngine;
using System.Threading.Tasks;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Screen Reader Test")]
	public class TtsTest : ScriptableCommand
	{
		[SerializeField]
		private ScreenReaderHolder screenReader = null!;
		
		/// <inheritdoc />
		protected override async Task OnExecute()
		{
			if (screenReader == null || screenReader.Value == null)
			{
				Console.WriteLine("Speech service not active.");
				this.EndProcess();
			}

			var textBuilder = new StringBuilder();

			while (Console.ReadLine(out string line))
				textBuilder.AppendLine(line);
			
			string text = string.Join(' ', Arguments);
			textBuilder.AppendLine(text);

			await screenReader.Value.Speak(textBuilder.ToString());
			EndProcess();
		}
	}
}