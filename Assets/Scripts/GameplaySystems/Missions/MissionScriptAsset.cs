﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Architecture;
using Core;
using Core.Scripting;
using Core.Scripting.Instructions;
using GamePlatform;
using OS.Devices;
using UnityEngine;
using Missions;
using Social;
using UI.Shell;

namespace GameplaySystems.Missions
{
	public class MissionScriptAsset : 
		ScriptableObject,
		IMission,
		ICachedScript
	{
		[SerializeField]
		private string missionName = string.Empty;

		[SerializeField]
		private string narrativeIdOfGiver = string.Empty;

		[SerializeField]
		private DangerLevel dangerLevel;

		[SerializeField]
		private MissionType type;

		[SerializeField]
		private MissionStartCondition startCondition;

		[SerializeField]
		private List<string> requiredMissions = new List<string>();
		
		[SerializeField]
		private SerializableDictionary<string, string> defaultVariables = new SerializableDictionary<string, string>();
		
		[TextArea]
		[SerializeField]
		private string scriptText = string.Empty;

		[NonSerialized]
		private ShellInstruction? startTree;

		[NonSerialized]
		private ShellInstruction? emailTree;
		
		#if UNITY_EDITOR
		public void SetScriptText(string text)
		{
			this.scriptText = text;
		}

		public void ImportMetadata()
		{
			defaultVariables.Clear();
			
			var importContext = new MissionScriptImportContext(this);
			var shell = new InteractiveShell((importContext));

			var scriptBuilder = new StringBuilder();
			scriptBuilder.AppendLine(this.scriptText);
			scriptBuilder.AppendLine("metadata");

			shell.Setup(new UnityTextConsole());
			shell.RunScript(scriptBuilder.ToString()).Wait();
		}

		private class MissionScriptImportContext : IScriptExecutionContext
		{
			private readonly ScriptFunctionManager functions = new();
			private readonly MissionScriptAsset asset;

			public MissionScriptImportContext(MissionScriptAsset asset)
			{
				this.asset = asset;
			}
			
			/// <inheritdoc />
			public string Title => "Mission Importer";

			/// <inheritdoc />
			public string GetVariableValue(string variableName)
			{
				if (!asset.defaultVariables.TryGetValue(variableName, out string value))
					return string.Empty;

				return value;
			}

			/// <inheritdoc />
			public void SetVariableValue(string variableName, string value)
			{
				asset.defaultVariables[variableName] = value;
			}

			/// <inheritdoc />
			public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
			{
				int? functionResult = await functions.CallFunction(name, args, console, callSite ?? this);
				if (functionResult != null)
					return functionResult;
				
				switch (name)
				{
					case "export":
						if (args.Length == 1)
						{
							string varName = args[0];
							string value = callSite?.GetVariableValue(varName) ?? string.Empty;
							asset.defaultVariables[varName] = value;
						}
						else if (args.Length == 3)
						{
							string varName = args[0];
							string value = args[2];
				
							SetVariableValue(varName, value);
						}
						return 0;
					case "name":
						asset.missionName = string.Join(" ", args);
						return 0;
					case "giver":
						asset.narrativeIdOfGiver = string.Join(" ", args);
						return 0;
					case "danger":
						if (args.Length != 1)
							throw new InvalidOperationException("Mission metadata error: Danger directive expects one argument with a valid integer.");

						if (!int.TryParse(args[0], out int rawDanger))
							throw new InvalidOperationException("Mission metadata error: Danger value must be a valid integer.");

						asset.dangerLevel = rawDanger switch
						{
							{ } when rawDanger < 1 => DangerLevel.None,
							1 => DangerLevel.Minor,
							2 => DangerLevel.Moderate,
							_ => DangerLevel.Dangerous
						};
						return 0;
					case "type":
						if (args.Length != 1)
							throw new InvalidOperationException("Mission metadata error: type directive expects one argument.");

						if (!Enum.TryParse(args[0], true, out MissionType missionType))
							throw new InvalidOperationException("Failed to parse mission type. ");

						asset.type = missionType;
						return 0;
					case "start_type":
						if (args.Length != 1)
							throw new InvalidOperationException("Mission metadata error: start_type directive expects one argument.");
						
						if (!Enum.TryParse(args[0], true, out MissionStartCondition startCondition))
							throw new InvalidOperationException("Failed to parse mission start type. ");

						asset.startCondition = startCondition;
						return 0;
					case "requires":
						asset.requiredMissions.Clear();
						asset.requiredMissions.AddRange(args);
						return 0;
					default:
						return null;
				}
			}

			/// <inheritdoc />
			public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
			{
				if (mode != FileRedirectionType.None)
					Debug.LogWarning("File redirection used while importing a mission script. This is not supported.");

				return realConsole;
			}

			/// <inheritdoc />
			public void HandleCommandNotFound(string name, string[] args, ITextConsole console)
			{
				throw new InvalidOperationException($"Command {name} not found or not supported during mission script import. Make sure you are not running any commands in the top-level scope of the script.");
			}

			/// <inheritdoc />
			public void DeclareFunction(string name, IScriptFunction body)
			{
				functions.DeclareFunction(name, body);
			}
		}
		
		#endif
		/// <inheritdoc />
		public string Id => name;

		/// <inheritdoc />
		public string Name => missionName;

		/// <inheritdoc />
		public DangerLevel DangerLevel => dangerLevel;

		/// <inheritdoc />
		public MissionType Type => type;

		/// <inheritdoc />
		public MissionStartCondition StartCondition => startCondition;

		/// <inheritdoc />
		public string GiverId => narrativeIdOfGiver;

		/// <inheritdoc />
		public bool IsAvailable(IWorld world)
		{
			return this.requiredMissions.All(world.IsMissionCompleted);
		}

		/// <inheritdoc />
		public bool IsCompleted(IWorld world)
		{
			return world.IsMissionCompleted(Id);
		}

		/// <inheritdoc />
		public async Task<string> GetBriefingText(IProfile playerProfile)
		{
			if (this.emailTree == null)
				await this.RebuildScriptTree();
			
			var stringConsole = new StringConsole();

			var scriptContext = new UserScriptExecutionContext();
			
			scriptContext.SetVariableValue("PLAYER_NAME", playerProfile.ChatName);
			scriptContext.SetVariableValue("PLAYER_USERNAME", playerProfile.ChatUsername);
			
			var shell = new InteractiveShell(scriptContext);
			
			shell.Setup(stringConsole);
			await shell.RunParsedScript(emailTree);
			
			return stringConsole.GetText();
		}

		/// <inheritdoc />
		public async Task StartMission(IMissionController missionController, CancellationToken cancellationToken)
		{
			if (this.startTree == null)
				await RebuildScriptTree();
			
			var scriptContext = new MissionScriptContext(missionController, this);
			var console = new UnityTextConsole();
			var shell = new InteractiveShell(scriptContext);
			
			// TODO: Allow the script to be cancelled via mission abandonment token
			missionController.DisableAbandonment();
			
			shell.Setup(console);
			await shell.RunParsedScript(startTree);
			
			missionController.EnableAbandonment();
		}

		/// <inheritdoc />
		public async Task RebuildScriptTree()
		{
			var context = new UserScriptExecutionContext();
			var console = new UnityTextConsole();
			var shell = new InteractiveShell(context);
			
			shell.Setup(console);

			ShellInstruction primaryTree = await shell.ParseScript(this.scriptText);

			this.startTree = CreateStartTree(primaryTree, "start");
			this.emailTree = CreateStartTree(primaryTree, "email");
		}

		private ShellInstruction CreateStartTree(ShellInstruction primaryTree, string entrypoint)
		{
			var instructionList = new List<ShellInstruction>();
			instructionList.Add(primaryTree);

			var nameEvaulator = new TextArgumentEvaluator(entrypoint);
			var command = new CommandData(nameEvaulator, Enumerable.Empty<IArgumentEvaluator>(), FileRedirectionType.None, null);
			var single = new SingleInstruction(command);
			
			instructionList.Add(single);

			return new SequentialInstruction(instructionList);
		}
	}
}