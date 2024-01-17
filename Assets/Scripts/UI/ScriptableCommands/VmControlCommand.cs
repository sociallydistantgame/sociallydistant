#nullable enable
using System;
using Architecture;
using UnityEngine;
using Utility;
using System.Threading.Tasks;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Virtual Machine Control")]
	public class VmControlCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			if (this.Arguments.Length < 1)
			{
				PrintUsage();
				return Task.CompletedTask;
			}

			string command = Arguments[0];

			switch (command)
			{
				case "poweroff":
					PlatformHelper.QuitToDesktop();
					break;
				default:
					PrintUsage();
					break;
			}
			
			return Task.CompletedTask;
		}

		private void PrintUsage()
		{
			Console.WriteLine(@"vmctl: usage: vmctl <command>

Commands:
    poweroff: Exits Socially Distant, quitting to the desktop.");
		}
	}
}