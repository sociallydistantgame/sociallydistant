#nullable enable
using System;
using System.Threading.Tasks;
using Core.Scripting;
using GameplaySystems.Chat;
using OS.Devices;
using System.Collections.Generic;
using System.Linq;
using Chat;

namespace Editor.CustomImporters
{
	public sealed class ChatImportContext : IScriptExecutionContext
	{
		private readonly ChatConversationAsset asset;
		private readonly Dictionary<string, string> variables = new();
		private readonly ScriptFunctionManager functions = new();

		/// <inheritdoc />
		public string Title => asset.Id;

		public ChatImportContext(ChatConversationAsset asset)
		{
			this.asset = asset;
		}

		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			if (!variables.TryGetValue(variableName, out string value))
				return string.Empty;
			
			return value;
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

			switch (name)
			{
				case "type":
					if (args.Length < 1)
						throw new InvalidOperationException("type expects one argument");

					if (!Enum.TryParse(args[0], true, out ChatScriptType type))
						throw new InvalidOperationException($"Could not parse {args[0]} into a valid {nameof(ChatScriptType)} enum value");

					asset.Type = type;
					return 0;
				case "start_type":
					if (args.Length < 1)
						throw new InvalidOperationException("start_type expects one argument");

					if (!Enum.TryParse(args[0], true, out ChatStartType startType))
						throw new InvalidOperationException($"Could not parse {args[0]} into a valid {nameof(ChatStartType)} enum value");

					asset.StartType = startType;
					return 0;
				case "actor":
					foreach (string narrativeId in args)
					{
						asset.AddActor(narrativeId);
					}
					return 0;
				case "start_message":
					string startMessage = string.Join(" ", args);
					asset.StartMessage = startMessage;
					return 0;
				case "guild":
					string guild = string.Join(" ", args);
					asset.GuildId = guild;
					return 0;
				case "channel":
					string channel = string.Join(" ", args);
					asset.ChannelId = channel;
					return 0;
				case "repeatable":
					if (args.Length < 1)
						throw new InvalidOperationException("repeatable expects one argument");

					if (!bool.TryParse(args[0], out bool repeatable))
						throw new InvalidOperationException($"Could not parse {args[0]} into a valid boolean value");

					asset.IsRepeatable = repeatable;
					return 0;
				case "condition":
					if (args.Length < 2)
						throw new InvalidOperationException($"condition directives in a chat script require at least 2 parameters.");

					string meetType = args[0];
					string checkType = args[1];
					string[] conditionParams = args.Skip(2).ToArray();

					if (!Enum.TryParse(meetType, true, out ScriptConditionType conditionType))
						throw new InvalidOperationException($"condition: parameter 1: {meetType}: must be either 'met' or 'unmet'");

					if (!Enum.TryParse(checkType, true, out ScriptConditionCheck conditionCheck))
						throw new InvalidOperationException($"condition: parameter 2: {checkType}: must be a valid script condition type");
					
					asset.AddCondition(new ConversationScriptCondition
					{
						Type = conditionType,
						Check = conditionCheck,
						Parameters = conditionParams
					});
					
					return 0;
				case "conditions":
				{
					if (args.Length < 1)
						throw new InvalidOperationException("conditions: usage: conditions <type> [parameter]");

					string rawType = args[0];

					if (!Enum.TryParse(rawType, true, out ScriptConditionMode mode))
						throw new InvalidOperationException($"Could not parse {rawType} into a ScriptConditionMode value.");

					asset.ConditionsMode = mode;

					if (args.Length > 1)
					{
						string param = args[1];
						if (int.TryParse(param, out int value))
							asset.ConditionModeParameter = value;
					}
					
					return 0;
				}
				default:
					return null;
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
			throw new InvalidOperationException($"{this.Title}: {name}: Command not found.");
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			this.functions.DeclareFunction(name, body);
		}
	}
}