#nullable enable
using Architecture;
using Core;
using UnityEngine;
using Utility;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Make Directory")]
	public class MakeDirectoryCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override void OnExecute()
		{
			if (Arguments.Length == 0)
			{
				Console.WriteLine("mkdir: usage: cat <filepath>");
				return;
			}

			string fullPath = PathUtility.Combine(CurrentWorkingDirectory, Arguments[0]);
			
			if (!FileSystem.DirectoryExists(fullPath))
				FileSystem.CreateDirectory(fullPath);
		}
	}
}