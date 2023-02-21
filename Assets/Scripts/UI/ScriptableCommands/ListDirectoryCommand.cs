#nullable enable

using Architecture;
using UnityEngine;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/List Directory")]
	public class ListDirectoryCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override void OnExecute()
		{
			Console.WriteLine("TBD");
		}
	}
}