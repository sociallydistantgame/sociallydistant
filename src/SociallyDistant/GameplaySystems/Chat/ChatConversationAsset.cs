#nullable enable

using SociallyDistant.Core.Chat;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.Scripting.Instructions;
using SociallyDistant.Core.Core.Scripting.Parsing;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Social;
using SociallyDistant.GamePlatform;

namespace SociallyDistant.GameplaySystems.Chat
{
	public sealed class ChatConversationAsset : ShellScriptAsset,
		IChatConversation,
		ICachedScript
	{
		private readonly ScriptMetadata    scriptMetadata = new();
		private readonly string            scriptText;
		private readonly string            id;
		private          ShellInstruction? scriptTree;

		internal ChatConversationAsset(string id, string text)
		{
			this.id = id;
			this.scriptText = text;
		}

		/// <inheritdoc />
		public string Id => id;

		/// <inheritdoc />
		public IEnumerable<string> ActorIds => scriptMetadata.actorIds;

		/// <inheritdoc />
		public ChatScriptType Type => scriptMetadata.scriptType;


		public ScriptConditionMode ConditionsMode => scriptMetadata.policy;

		public int ConditionModeParameter => scriptMetadata.policyParameter;

		/// <inheritdoc />
		public string ChannelId => scriptMetadata.channelId;

		/// <inheritdoc />
		public string GuildId => scriptMetadata.guildId;

		/// <inheritdoc />
		public ChatStartType StartType => scriptMetadata.startType;

		/// <inheritdoc />
		public bool CheckConditions(IWorldManager world, ISocialService socialService)
		{
			if (this.Type == ChatScriptType.Guild)
			{
				if (string.IsNullOrWhiteSpace(ChannelId) || string.IsNullOrWhiteSpace(GuildId))
					return false;
			}

			if (this.Type == ChatScriptType.Group)
			{
				if (string.IsNullOrWhiteSpace(ChannelId))
					return false;
			}

			var uniqueTypes = new List<ScriptConditionCheck>();

			foreach (ConversationScriptCondition? condition in scriptMetadata.conditions)
			{
				bool checkResult = condition.CheckCondition(world, socialService);

				if (checkResult && scriptMetadata.policy == ScriptConditionMode.Any)
					return true;

				if (!checkResult && scriptMetadata.policy == ScriptConditionMode.All)
					return false;

				if (checkResult)
				{
					if (!uniqueTypes.Contains(condition.Check))
						uniqueTypes.Add(condition.Check);
				}
			}

			if (scriptMetadata.policy == ScriptConditionMode.AtLeast)
				return uniqueTypes.Count >= scriptMetadata.policyParameter;

			return true;
		}

		/// <inheritdoc />
		public async Task StartConversation(CancellationToken token, IConversationController controller)
		{
			if (this.scriptTree == null)
				await this.RebuildScriptTree();

			var context = new ChatScriptContext(controller);
			var console = new UnityTextConsole();

			var shell = new InteractiveShell(context);

			shell.Setup(console);

			// TODO: Cancellation!!
			int result = await shell.RunParsedScript(this.scriptTree);
			// TODO: Do something with the result
		}

		/// <inheritdoc />
		public bool IsRepeatable => scriptMetadata.repeatable;

		/// <inheritdoc />
		public string StartMessage => scriptMetadata.startMessage;

		private void AddActor(string narrativeId)
		{
			if (this.scriptMetadata.actorIds.Contains(narrativeId))
				return;

			this.scriptMetadata.actorIds.Add(narrativeId);
		}

		private void AddCondition(ConversationScriptCondition condition)
		{
			scriptMetadata.conditions.Add(condition);
		}

		/// <inheritdoc />
		public async Task RebuildScriptTree()
		{
			var context = new UserScriptExecutionContext();
			var console = new UnityTextConsole();

			var shell = new InteractiveShell(context);
			shell.Setup(console);

			var mainParseTree = await shell.ParseScript(this.scriptText);
			var metadataTree = new SequentialInstruction(new[] { mainParseTree, new SingleInstruction(new CommandData(new TextArgumentEvaluator("metadata"), Enumerable.Empty<IArgumentEvaluator>(), FileRedirectionType.None, null)) });

			var metadataShell = new InteractiveShell(new ChatImportContext(this));
			metadataShell.Setup(console);

			await metadataShell.RunParsedScript(metadataTree);
			
			this.scriptTree = new SequentialInstruction(new[] { mainParseTree, new SingleInstruction(new CommandData(new TextArgumentEvaluator("main_branch"), Enumerable.Empty<IArgumentEvaluator>(), FileRedirectionType.None, null)) });
		}

		private class ScriptMetadata
		{
			public List<string>                      actorIds = new();
			public ChatScriptType                    scriptType;
			public string                            guildId = string.Empty;
			public ChatStartType                     startType;
			public string                            startMessage = string.Empty;
			public bool                              repeatable   = true;
			public string                            channelId    = string.Empty;
			public List<ConversationScriptCondition> conditions   = new();
			public ScriptConditionMode               policy;
			public int                               policyParameter;
		}

		private sealed class ChatImportContext : IScriptExecutionContext
		{
			private readonly ChatConversationAsset      asset;
			private readonly Dictionary<string, string> variables = new();
			private readonly ScriptFunctionManager      functions = new();

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
			public async Task<int?> TryExecuteCommandAsync(
				string name,
				string[] args,
				ITextConsole console,
				IScriptExecutionContext? callSite = null
			)
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

						asset.scriptMetadata.scriptType = type;
						return 0;
					case "start_type":
						if (args.Length < 1)
							throw new InvalidOperationException("start_type expects one argument");

						if (!Enum.TryParse(args[0], true, out ChatStartType startType))
							throw new InvalidOperationException($"Could not parse {args[0]} into a valid {nameof(ChatStartType)} enum value");

						asset.scriptMetadata.startType = startType;
						return 0;
					case "actor":
						foreach (string narrativeId in args)
						{
							asset.AddActor(narrativeId);
						}

						return 0;
					case "start_message":
						string startMessage = string.Join(" ", args);
						asset.scriptMetadata.startMessage = startMessage;
						return 0;
					case "guild":
						string guild = string.Join(" ", args);
						asset.scriptMetadata.guildId = guild;
						return 0;
					case "channel":
						string channel = string.Join(" ", args);
						asset.scriptMetadata.channelId = channel;
						return 0;
					case "repeatable":
						if (args.Length < 1)
							throw new InvalidOperationException("repeatable expects one argument");

						if (!bool.TryParse(args[0], out bool repeatable))
							throw new InvalidOperationException($"Could not parse {args[0]} into a valid boolean value");

						asset.scriptMetadata.repeatable = repeatable;
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

						asset.AddCondition(new ConversationScriptCondition { Type = conditionType, Check = conditionCheck, Parameters = conditionParams });

						return 0;
					case "conditions":
					{
						if (args.Length < 1)
							throw new InvalidOperationException("conditions: usage: conditions <type> [parameter]");

						string rawType = args[0];

						if (!Enum.TryParse(rawType, true, out ScriptConditionMode mode))
							throw new InvalidOperationException($"Could not parse {rawType} into a ScriptConditionMode value.");

						asset.scriptMetadata.policy = mode;

						if (args.Length > 1)
						{
							string param = args[1];
							if (int.TryParse(param, out int value))
								asset.scriptMetadata.policyParameter = value;
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
}