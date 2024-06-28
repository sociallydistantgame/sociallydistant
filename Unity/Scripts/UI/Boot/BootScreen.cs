#nullable enable

using System;
using System.Linq;
using OS.Devices;
using UI.Terminal.SimpleTerminal;
using UnityEngine;
using UnityExtensions;
using System.Threading.Tasks;
using Core;
using Core.Config;
using GamePlatform;
using UI.PlayerUI;

namespace UI.Boot
{
	public class BootScreen : MonoBehaviour
	{
		
		private SimpleTerminalRenderer terminal = null!;

		private Task? bootTask = null;

		private readonly string assCoveringText = @"This game saves data automatically to your device.

Socially Distant and the associated logo are copyright © 2022 Michael VanOverbeek. All rights 
reserved. The acidic light, acidic light community, and 'Ritchie' logos are © 2022 Michael
VanOverbeek. All rights reserved. Further license information may be found in the About
section of System Settings.

Use of online features within this game are subject to the acidic light community code of
conduct. Violation may result in restriction or termination of access to online functionality.
Direct all inquiries to https://sociallydistantgame.com/support.";

		private readonly string devInfo = @"You are playing an incomplete version of Socially Distant.
Any content in this build is subject to change and does not reflect
the final product.";

		private bool skipAssCovering;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(BootScreen));
		}

		private void Start()
		{
			var modSettings = new ModdingSettings(GameManager.Instance.SettingsManager);
			var uiSettings = new UiSettings(GameManager.Instance.SettingsManager);

			skipAssCovering = modSettings.ModDebugMode || uiSettings.SkipAssCovering;
			
			ITextConsole console = terminal.StartSession();

			var driver = new ConsoleWrapper(console);

			this.bootTask = DoGameBoot(driver);
		}

		private async Task PrintCenteredMessages(ConsoleWrapper console, params string[] messages)
		{
			console.UseAltScreen(true);
			console.Clear();

			int width = messages.Select(x => x.Length).Max();
			int x = ((terminal.Columns - width) / 2);

			bool waitOnWords = messages.Length > 1;

			var beep = true;
			for (var i = 0; i < messages.Length; i++)
			{
				int y = ((terminal.Rows - messages.Length) / 2) + i;

				console.SetCursorPos(x, y);

				string message = messages[i];
				for (var j = 0; j < message.Length; j++)
				{
					bool wait = !waitOnWords || char.IsWhiteSpace(message[j]);

					if (wait)
					{
						if (beep)
							console.Write("\a");
						await Task.Delay(20);

						beep = !beep;
					}

					console.Write($"{message[j]}");
				}
			}

			await Task.Delay(5000);

			console.UseAltScreen(false);
			console.WriteWithTimestamp("Terminal capability test complete.");

		}
		
		private async Task DoGameBoot(ConsoleWrapper console)
		{
			console.WriteWithTimestamp("Initializing video driver... done");
			await Task.Delay(260);
			console.WriteWithTimestamp("Testing terminal capabilities...");

			await Task.Delay(420);

			if (!skipAssCovering)
			{
				await PrintCenteredMessages(console, assCoveringText.Split('\n'));
				await PrintCenteredMessages(console, devInfo.Split('\n'));
			}

			Application.logMessageReceived += LogMessageReceived;
			
			await GameManager.Instance.WaitForModulesToLoad();
			
			
			void LogMessageReceived(string condition, string stacktrace, LogType type)
			{
				console.WriteWithTimestamp(condition);
			}

			console.WriteWithTimestamp("Initializing kernel modules.... done");

			console.WriteWithTimestamp("Loading userspace...");

			GameManager.Instance.SettingsManager.Load();
			
			await GameManager.Instance.ContentManager.RefreshContentDatabaseAsync();

			console.WriteWithTimestamp("Starting X server...");

			await GameManager.Instance.DoUserInitialization();
		}
	}
}