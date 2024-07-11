#nullable enable

using SociallyDistant.Core.Chat;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.Scripting.Instructions;
using SociallyDistant.Core.Social;
using SociallyDistant.GamePlatform;

namespace SociallyDistant.GameplaySystems.Chat
{
	public sealed class ChatConversationAsset : ShellScriptAsset,
		IChatConversation,
		ICachedScript
	{
		private readonly string                            scriptText;
		private readonly string                            id;
		private readonly List<string>                      actorIds = new();
		private readonly ChatScriptType                    scriptType;
		private readonly string                            guildId = string.Empty;
		private readonly ChatStartType                     startType;
		private readonly string                            startMessage = string.Empty;
		private readonly bool                              repeatable   = true;
		private readonly string                            channelId    = string.Empty;
		private readonly List<ConversationScriptCondition> conditions   = new();
		private readonly ScriptConditionMode               policy;
		private readonly int                               policyParameter;
		private          ShellInstruction?                 scriptTree;

		internal ChatConversationAsset(string id, string text)
		{
			this.id = id;
			this.scriptText = text;
		}

		/// <inheritdoc />
		public string Id => id;
		
		/// <inheritdoc />
		public IEnumerable<string> ActorIds => actorIds;

		/// <inheritdoc />
		public ChatScriptType Type => scriptType;


		public ScriptConditionMode ConditionsMode => policy;

		public int ConditionModeParameter => policyParameter;

		/// <inheritdoc />
		public string ChannelId => channelId;

		/// <inheritdoc />
		public string GuildId => guildId;

		/// <inheritdoc />
		public ChatStartType StartType => startType;

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
			
			foreach (ConversationScriptCondition? condition in conditions)
			{
				bool checkResult = condition.CheckCondition(world, socialService);

				if (checkResult && policy == ScriptConditionMode.Any)
					return true;

				if (!checkResult && policy == ScriptConditionMode.All)
					return false;

				if (checkResult)
				{
					if (!uniqueTypes.Contains(condition.Check))
						uniqueTypes.Add(condition.Check);
				}
			}

			if (policy == ScriptConditionMode.AtLeast)
				return uniqueTypes.Count >= policyParameter;
			
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
		public bool IsRepeatable => repeatable;

		/// <inheritdoc />
		public string StartMessage => startMessage;
		
		public void AddActor(string narrativeId)
		{
			if (this.actorIds.Contains(narrativeId))
				return;

			this.actorIds.Add(narrativeId);
		}

		public void AddCondition(ConversationScriptCondition condition)
		{
			conditions.Add(condition);
		}
		
		/// <inheritdoc />
		public async Task RebuildScriptTree()
		{
			var context = new UserScriptExecutionContext();
			var console = new UnityTextConsole();

			var shell = new InteractiveShell(context);
			shell.Setup(console);

			this.scriptTree = await shell.ParseScript(this.scriptText + Environment.NewLine + "main_branch");
		}
	}
}