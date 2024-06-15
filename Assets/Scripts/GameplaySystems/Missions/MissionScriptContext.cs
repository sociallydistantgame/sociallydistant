#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Scripting;
using Core.Scripting.GlobalCommands;
using Core.Scripting.StandardModules;
using Cysharp.Threading.Tasks.Triggers;
using Missions;
using OS.Devices;
using UnityEngine;

namespace GameplaySystems.Missions
{
	public sealed class MissionScriptContext : IScriptExecutionContext
	{
		private readonly ScriptModuleManager modules = new();
		private readonly ScriptFunctionManager functions = new();
		private readonly Dictionary<string, string> variables = new(0);
		private readonly IMission mission;
		private readonly IMissionController missionController;

		public MissionScriptContext(IMissionController controller, IMission mission)
		{
			this.missionController = controller;
			this.mission = mission;

			this.modules.RegisterModule(new ShellHelpersModule(missionController.Game));
			this.modules.RegisterModule(new NpcModule(missionController.Game.SocialService));
		}

		/// <inheritdoc />
		public string Title => mission.Name;

		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			if (variables.TryGetValue(variableName, out string value))
				return value;

			return string.Empty;
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
			variables[variableName] = value;
		}

		/// <inheritdoc />
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			int? functionResult = await functions.CallFunction(name, args, console, callSite ?? this);
			if (functionResult != null)
				return functionResult;

			int? moduleResult = await modules.TryExecuteFunction(name, args, console, callSite ?? this);
			if (moduleResult != null)
				return moduleResult;
			
			switch (name)
			{
				case "objective":
				case "challenge":
				{
					if (args.Length < 1)
						throw new InvalidOperationException($"{name}: Invalid objective/challenge directive. At least one argument is required to specify the objective type.");

					if (!Enum.TryParse(args[0], true, out ObjectiveType objectiveType))
						throw new InvalidOperationException($"{name}: Invalid objective/challenge directive. Failed to parse objective type. Unrecognized type {args[0]}.");
					
					bool isChallenge = name == "challenge";
					string[] parameters = args.Skip(1).ToArray();

					await HandleObjective(callSite ?? this, console, objectiveType, isChallenge, parameters);
					return 0;
				}
				case "worldflag":
				{
					var worldFlagCommand = new WorldFlagCommand(this.missionController.WorldManager);

					await worldFlagCommand.ExecuteAsync(callSite ?? this, console, name, args);
					return 0;
				}
				case "export":
				{
					return 0;
				}
			}

			if (callSite != null && callSite != this)
				return await callSite.TryExecuteCommandAsync(name, args, console, this);

			return null;
		}

		private async Task HandleObjective(IScriptExecutionContext context, ITextConsole console, ObjectiveType objectiveType, bool isChallenge, string[] parameters)
		{
			IObjectiveHandle handle = missionController.CreateObjective(string.Empty, string.Empty, isChallenge);

			switch (objectiveType)
			{
				case ObjectiveType.Scripted:
				{
					if (parameters.Length < 1)
						throw new InvalidOperationException("Scripted objectives require at least one parameter to specify the command to run.");

					string commandName = parameters[0];
					string[] commandArgs = parameters.Skip(1).ToArray();
					
					// create the objective context so the script system has access to objective-specific commands
					var objectiveContext = new ObjectiveScriptContext(context, handle);
					
					// Run the command and wait. 
					int? result = await objectiveContext.TryExecuteCommandAsync(commandName, commandArgs, console, this);
					
					// if we got a null, the command wasn't found
					if (result == null)
					{
						HandleCommandNotFound(commandName, commandArgs, console);
						return;
					}
					
					// if we get a 0 and the objective isn't already failed, we can complete it
					if (result == 0 && !handle.IsFAiled)
						handle.MarkCompleted();

					break;
				}
				default:
				{
					Debug.LogError($"Unrecognized objective type {objectiveType}. Mission script is likely written for a different build of the game. Objective will be marked as completed.");
					handle.MarkCompleted();
					break;
				}
			}
		}
		
		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			return realConsole;
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, string[] args, ITextConsole console)
		{
			throw new InvalidOperationException($"{Title}: {name}: Command not found. Mission will be forcibly abandoned and game will be reset.");
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			functions.DeclareFunction(name, body);
		}
	}
}