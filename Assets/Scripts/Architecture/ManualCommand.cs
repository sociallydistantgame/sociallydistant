#nullable enable
using System;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/Commands/Manual Command")]
	public class ManualCommand : ScriptableCommand
	{
		[SerializeField]
		private string manualUrl = "https://man.sociallydistantgame.com/";
		
		/// <inheritdoc />
		protected override void OnExecute()
		{
			System.Diagnostics.Process.Start(manualUrl);
		}
	}
}