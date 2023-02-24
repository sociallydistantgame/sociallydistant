#nullable enable
using System;
using Architecture;
using UnityEngine;
using Utility;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Virtual Machine Control")]
	public class VmControlCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override void OnExecute()
		{
			if (this.Arguments.Length < 1)
			{
				PrintUsage();
				return;
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
		}

		private void PrintUsage()
		{
			Console.WriteLine(@"vmctl: usage: vmctl <command>

Commands:
    poweroff: Exits Socially Distant, quitting to the desktop.");
		}
	}
}