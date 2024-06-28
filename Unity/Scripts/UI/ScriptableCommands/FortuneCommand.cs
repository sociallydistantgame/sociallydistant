#nullable enable
using Architecture;
using Misc.Fortune;
using UnityEngine;
using System.Threading.Tasks;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Fortune")]
	public class FortuneCommand : ScriptableCommand
	{
		
		private FortunesTable fortunesTable = null!;
		
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			Console.WriteLine(fortunesTable.GetRandomFortune().TrimEnd());
			return Task.CompletedTask;
		}
	}
}