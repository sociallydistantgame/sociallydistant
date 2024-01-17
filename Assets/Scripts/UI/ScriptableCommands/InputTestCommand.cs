#nullable enable
using System.Threading.Tasks;
using Architecture;
using OS.Devices;
using UnityEngine;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Input Test")]
	public class InputTestCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override async Task OnExecute()
		{
			var running = true;

			Console.WriteLine("Terminal input tester");
			Console.WriteLine();
			Console.WriteLine("Use this tool to test device inputs in your terminal. When an input is received, details will be printed to the screen. To exit, press the Escape key.");
			
			while (running)
			{
				ConsoleInputData input = await Console.ReadKeyAsync();

				if (input.KeyCode == KeyCode.Escape && !input.HasModifiers)
					running = false;

				if (input.Character == '\0')
				{
					Console.Write("[keyboard] ");

					if (input.Modifiers.HasFlag(KeyModifiers.Control))
						Console.Write("<control>");
					if (input.Modifiers.HasFlag(KeyModifiers.Alt))
						Console.Write("<alt>");
					if (input.Modifiers.HasFlag(KeyModifiers.Shift))
						Console.Write("<shift>");
					
					
					Console.WriteLine(input.KeyCode.ToString());
				}
				else
				{
					Console.WriteLine($"[char] '{input.Character}'");
				}
			}
		}
	}
}