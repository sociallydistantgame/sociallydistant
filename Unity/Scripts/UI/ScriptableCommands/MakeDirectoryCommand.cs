#nullable enable
using Architecture;
using Core;
using UnityEngine;
using Utility;
using System.Threading.Tasks;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Make Directory")]
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
	}
}