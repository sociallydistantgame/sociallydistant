#nullable enable
using System.Text;
using Accessibility;
using Architecture;
using UnityEngine;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Screen Reader Test")]
	public class TtsTest : ScriptableCommand
	{
		[SerializeField]
		private ScreenReaderHolder screenReader = null!;
		
		/// <inheritdoc />
		protected override async void OnExecute()
		{
			if (screenReader == null || screenReader.Value == null)
			{
				Console.WriteLine("Speech service not active.");
				this.EndProcess();
				return;
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