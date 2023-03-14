#nullable enable
using Architecture;
using Misc.Fortune;
using UnityEngine;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Fortune")]
	public class FortuneCommand : ScriptableCommand
	{
		[SerializeField]
		private FortunesTable fortunesTable = null!;
		
		/// <inheritdoc />
		protected override void OnExecute()
		{
			Console.WriteLine(fortunesTable.GetRandomFortune().TrimEnd());
		}
	}
}