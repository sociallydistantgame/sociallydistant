#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Architecture;
using Core.Config;
using GamePlatform;
using UnityEngine;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/System/Registry Control Command")]
	public class RegistryControlCommand : ScriptableCommand
	{
		[SerializeField]
		private GameManagerHolder gameManagerHolder = null!;
		
		/// <inheritdoc />
		protected override void OnExecute()
		{
			if (gameManagerHolder.Value == null)
			{
				Console.WriteLine("Could not connect to the System Settings service.");
				EndProcess();
				return;
			}
			
			if (Arguments.Length == 0)
			{
				PrintUsage();
				return;
			}

			ISettingsManager settings = gameManagerHolder.Value.SettingsManager;
            
			switch (Arguments[0])
			{
				case "reload":
					settings.Load();
					Console.WriteLine("The registry has been reloaded.");
					break;
				case "list-keys":
					ListKeys(settings);
					break;
				case "dump":
					DumpRegistryContents(settings);
					break;
				case "get" when Arguments.Length == 2:
				{
					string keyName = Arguments[1];
					if (settings.HasKey(keyName))
					{
						//string textValue = Registry.GetAllKeys().FirstOrDefault(x => x.Key == keyName)
							// .Value?.ToString() ?? "<null>";

						//Console.WriteLine($"{keyName}: {textValue}");
					}
					else
					{
						Console.WriteLine($"The given key {keyName} doesn't exist in the registry.");
					}
					break;
				}
				case "drop" when Arguments.Length == 2:
				{
					string keyName = Arguments[1];
					if (settings.HasKey(keyName))
					{
						settings.ResetKey(keyName);
						Console.WriteLine("Registry has been updated successfully.");
					}
					else
					{
						Console.WriteLine($"The key {keyName} wasn't found in the registry, no action has been taken.");
					}

					break;
				}
				default:
					PrintUsage();
					break;
			}
		}

		private void ListKeys(ISettingsManager settings)
		{
			foreach (string key in settings.Keys)
				Console.WriteLine(key);
		}
		
		private void DumpRegistryContents(ISettingsManager settings)
		{
			string[] keys = settings.Keys.ToArray();

			var typeNames = new List<string>();
			var textValues = new List<string>();

			foreach (string key in keys)
			{
				bool isBool = settings.IsBool(key);
				bool isString = settings.IsString(key);
				bool isFloat = settings.IsFloat(key);
				bool isInt = settings.IsInt(key);

				typeNames.Add(key switch
				{
					{ } when isBool => "bool",
					{ } when isFloat => "float",
					{ } when isString => "string",
					{ } when isInt => "int",
					_ => "unknown"
				});

				textValues.Add(key switch
				{
					{ } when isInt => settings.GetInt(key).ToString(),
					{ } when isBool => settings.GetBool(key).ToString(),
					{ } when isString => settings.GetString(key) ?? "<null>",
					{ } when isFloat => settings.GetFloat(key).ToString(),
					_ => "unknown"
				});
			}

			int longestKey = keys.Select(x => x.Length).OrderByDescending(x => x).First();
			int longestValue = textValues.Select(x => x.Length).OrderByDescending(x => x).First();
			
			Console.Write("Key");
			for (var i = 3; i <= longestKey; i++)
				Console.Write(" ");

			Console.WriteLine("Value");
			for (var i = 0; i <= Math.Max(longestKey, 3) + Math.Max(longestValue, 5); i++)
			{
				Console.Write("=");
			}

			Console.WriteLine();
			Console.WriteLine();

			for (var i = 0; i < keys.Length; i++)
			{
				string key = keys[i];
				string value = textValues[i];
				
				Console.Write(key);
				for (var j = key.Length; j <= longestKey;j++)
					Console.Write(" ");
				Console.WriteLine(value);
			}
		}
		
		private void PrintUsage()
		{
			Console.WriteLine($@"registryctl: Usage: registryctl <command> <arguments>

registryctl: Manages and reports the state of the Socially Distant settings registry.
Warning: Using registryctl is for advanced users only. Damaging the settings registry may
corrupt it, and in some cases, may result in the game crashing on startup.

Commands:
    reload:                     Reloads the registry from the settings file on disk. Will
                                wipe any unsaved settings.
    dump:                       Lists all keys currently stored in the registry, and their
                                values.
    list-keys:                  Lists all keys currently stored in the registry.
    get <key>:                  Prints the current value of the specified key.
    set-TYPE <key> <value>:     Sets the value of an existing key.
    drop <key>                  Deletes the specified existing key to the registry, 
                                resetting its value to the game's default.
    add-TYPE: <ley> <value>:    Adds a new key to the registry with the given value.

Registry key types
==================

If a command contains the word ""TYPE"", then it must be replaced with one of the following
supported .NET Framework value types.

	int:	Whole number value
	float:	Floating-point (fractional) number value
	string:	Arbitrary text
	bool:	Boolean value (true, false)
	
The registry may be manually edited outside of the game by editing the file:
{SettingsManager.RegistryFilePath}");
		}
	}
}