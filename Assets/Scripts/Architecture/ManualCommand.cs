#nullable enable
using System;
using UnityEngine;
using System.Threading.Tasks;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/Commands/Manual Command")]
	public class ManualCommand : ScriptableCommand
	{
		[SerializeField]
		private string manualUrl = "https://man.sociallydistantgame.com/";
		
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			System.Diagnostics.Process.Start(manualUrl);
			return Task.CompletedTask;
		}
	}
}