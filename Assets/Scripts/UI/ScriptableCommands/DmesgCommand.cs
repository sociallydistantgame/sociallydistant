#nullable enable
using System;
using System.Threading.Tasks;
using Architecture;
using UnityEditor;
using UnityEngine;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Kernel Message Command")]
	public sealed class DmesgCommand : ScriptableCommand
	{
		private static UnityLogReceiver log;
		
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			this.Console.WriteLine(log.GetLog());
			EndProcess();
			return Task.CompletedTask;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void SetupLogger()
		{
			log = new UnityLogReceiver();
		} 
	}
}