#nullable enable
using Architecture;
using Core;
using UnityEngine;
using Utility;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Cat")]
	public class CatCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override void OnExecute()
		{
			if (Arguments.Length == 0)
			{
				Console.WriteLine("cat: usage: cat <filepath>");
				return;
			}

			string filePath = PathUtility.Combine(CurrentWorkingDirectory, Arguments[0]);

			if (!FileSystem.FileExists(filePath))
			{
				Console.WriteLine($"cat: {filePath}: File not found.");
				return;
			}

			string fileText = FileSystem.ReadAllText(filePath);
			Console.WriteLine(fileText);
		}
	}
}