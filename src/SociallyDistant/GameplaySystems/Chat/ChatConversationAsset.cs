#nullable enable

using SociallyDistant.Core.Chat;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.Scripting.Instructions;
using SociallyDistant.Core.Social;
using SociallyDistant.GamePlatform;

namespace SociallyDistant.GameplaySystems.Chat
{
	public class ChatConversationAsset : 
		IChatConversation,
		ICachedScript
	{
		
		private string scriptText = string.Empty;

		
		private string id = string.Empty;

		
		private List<string> actorIds = new();

		
		private ChatScriptType scriptType;

		
		private string guildId = string.Empty;
		
		
		private ChatStartType startType;

		
		private string startMessage = string.Empty;

		
		private bool repeatable = true;

		
		private string channelId = string.Empty;

		
		private List<ConversationScriptCondition> conditions = new();

		
		private ScriptConditionMode policy;

		
		private int policyParameter;

		[NonSerialized]
		private ShellInstruction? scriptTree;

		/// <inheritdoc />
		public string Id
		{
			get => id;
			#if UNITY_EDITOR
			set => id = value;
#endif
		}

		/// <inheritdoc />
		public IEnumerable<string> ActorIds => actorIds;

		/// <inheritdoc />
		public ChatScriptType Type
		{
			get => scriptType;
#if UNITY_EDITOR
			set => scriptType = value;
#endif
		}


		public ScriptConditionMode ConditionsMode
		{
			get => policy;
#if UNITY_EDITOR
			set => policy = value;
#endif
		}

		public int ConditionModeParameter
		{
			get => policyParameter;
#if UNITY_EDITOR
			set => policyParameter = value;
#endif
		}

		/// <inheritdoc />
		public string ChannelId
		{
			get => channelId;
			#if UNITY_EDITOR
			set => this.channelId = value;
#endif
		}

		/// <inheritdoc />
		public string GuildId
		{
			get => guildId;
#if UNITY_EDITOR
			set => guildId = value;
#endif
		}

		/// <inheritdoc />
		public ChatStartType StartType
		{
			get => startType;
#if UNITY_EDITOR
			set => startType =  value;
#endif
		}

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
		public bool IsRepeatable
		{
			get  => repeatable;
#if UNITY_EDITOR
			set => repeatable = value;
#endif
		}

		/// <inheritdoc />
		public string StartMessage
		{
			get => startMessage;
#if UNITY_EDITOR
			set => startMessage = value;
#endif
		}
		
		#if UNITY_EDITOR

		public string ScriptText
		{
			get => scriptText;
			set => scriptText = value;
		}

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
		
		#endif
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