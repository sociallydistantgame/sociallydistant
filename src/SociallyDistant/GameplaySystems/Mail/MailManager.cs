#nullable enable

using System.Net.Mime;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Scripting;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.WorldData;
using SociallyDistant.GameplaySystems.Social;

namespace SociallyDistant.GameplaySystems.Mail
{
	public class MailManager : IHookListener
	{
		private MarkupAsset changelogMarkupAsset = null!;

		private static readonly Singleton<MailManager> singleton = new();
		
		private readonly Dictionary<ObjectId, MailThread> threads = new Dictionary<ObjectId, MailThread>();
		private readonly Dictionary<ObjectId, MailMessage> messages = new Dictionary<ObjectId, MailMessage>();
		
		private ISocialService socialService;
		private SociallyDistantGame gameManager = null!;
		private WorldManager worldManager = null!;
		
		private void Awake()
		{
			gameManager = SociallyDistantGame.Instance;
			worldManager = WorldManager.Instance;
			socialService = gameManager.SocialService;
			singleton.SetInstance(this);
		}

		private void Start()
		{
			gameManager.ScriptSystem.RegisterHookListener(CommonScriptHooks.AfterWorldStateUpdate, this);
			
			InstallEvents();
		}

		private void OnDestroy()
		{
			UninstallEvents();
			
			gameManager.ScriptSystem.UnregisterHookListener(CommonScriptHooks.AfterWorldStateUpdate, this);
			singleton.SetInstance(null);
		}

		private void InstallEvents()
		{
			worldManager.Callbacks.AddCreateCallback<WorldMailData>(OnCreateMail);
			worldManager.Callbacks.AddModifyCallback<WorldMailData>(OnModifyMail);
			worldManager.Callbacks.AddDeleteCallback<WorldMailData>(OnDeleteMail);
		}

		private void UninstallEvents()
		{
			worldManager.Callbacks.RemoveCreateCallback<WorldMailData>(OnCreateMail);
			worldManager.Callbacks.RemoveModifyCallback<WorldMailData>(OnModifyMail);
			worldManager.Callbacks.RemoveDeleteCallback<WorldMailData>(OnDeleteMail);
		}

		public IEnumerable<IMailMessage> GetMessagesForUser(IProfile profile)
		{
			foreach (MailMessage message in messages.Values)
			{
				if (message.To == profile)
					yield return message;
			}
		}
		
		public IEnumerable<IMailMessage> GetMessagesFromUser(IProfile profile)
		{
			foreach (MailMessage message in messages.Values)
			{
				if (message.From == profile)
					yield return message;
			}
		}
		
		private void GetThread(ObjectId threadId, out MailThread thread)
		{
			if (!threads.TryGetValue(threadId, out thread))
			{
				thread = new MailThread();
				threads.Add(threadId, thread);
			}
		}
		
		private void OnCreateMail(WorldMailData subject)
		{
			var message = new MailMessage(this);
			this.messages[subject.InstanceId] = message;

			GetThread(subject.ThreadId, out MailThread thread);
			thread.AddMessage(message);
			
			message.UpdateMessage(subject);
		}
		
		private void OnModifyMail(WorldMailData subjectprevious, WorldMailData subjectnew)
		{
			if (!messages.TryGetValue(subjectprevious.InstanceId, out MailMessage message))
				return;

			GetThread(subjectprevious.ThreadId, out MailThread oldThread);
			GetThread(subjectnew.ThreadId, out MailThread newThread);

			if (oldThread != newThread)
			{
				oldThread.RemoveMessage(message);
				newThread.AddMessage(message);

				if (oldThread.Count == 0)
				{
					threads.Remove(subjectprevious.ThreadId);
				}
			}
			
			message.UpdateMessage(subjectnew);

			if (subjectnew.InstanceId != subjectprevious.InstanceId)
			{
				messages.Remove(subjectprevious.InstanceId);
				messages[subjectnew.InstanceId] = message;
			}
			
			
		}
		
		private void OnDeleteMail(WorldMailData subject)
		{
			if (!messages.TryGetValue(subject.InstanceId, out MailMessage message))
				return;
            
			GetThread(subject.ThreadId, out MailThread thread);
			thread.RemoveMessage(message);

			messages.Remove(subject.InstanceId);

			if (thread.Count == 0)
				threads.Remove(subject.ThreadId);
		}
		
		private sealed class MailThread : IMailThread
		{
			private readonly List<MailMessage> messages = new List<MailMessage>();

			/// <inheritdoc />
			public int Count => messages.Count;

			/// <inheritdoc />
			public IEnumerable<IMailMessage> GetMessagesInThread()
			{
				return messages;
			}

			public void AddMessage(MailMessage message)
			{
				this.messages.Add(message);
			}

			public void RemoveMessage(MailMessage message)
			{
				this.messages.Remove(message);
			}
		}

		private sealed class MailMessage : IMailMessage
		{
			private readonly MailManager mailManager;

			/// <inheritdoc />
			public IProfile From { get; private set; }

			/// <inheritdoc />
			public string NarrativeId { get; private set; }
			
			/// <inheritdoc />
			public MailTypeFlags MessageType { get; private set; }
			
			/// <inheritdoc />
			public IProfile To { get; private set; }
			
			/// <inheritdoc />
			public IMailThread? Thread { get; private set; }
			
			/// <inheritdoc />
			public string Subject { get; private set; } = string.Empty;

			/// <inheritdoc />
			public IEnumerable<DocumentElement> Body { get; private set;  }


			public MailMessage(MailManager mailManager)
			{
				this.mailManager = mailManager;
			}
			
			public void UpdateMessage(WorldMailData mailData)
			{
				this.From = this.mailManager.socialService.GetProfileById(mailData.From);
				this.To = this.mailManager.socialService.GetProfileById(mailData.To);

				this.MessageType = mailData.TypeFlags;
				this.NarrativeId = mailData.NarrativeId;
				
				if (mailData.ThreadId.IsInvalid)
				{
					this.Thread = null;
				}
				else
				{
					mailManager.threads.TryGetValue(mailData.ThreadId, out MailThread? threadInternal);
					this.Thread = threadInternal;
				}

				this.Subject = mailData.Subject;
				this.Body = mailData.Document?.ToArray() ?? Array.Empty<DocumentElement>();
			}
		}

		/// <inheritdoc />
		public Task ReceiveHookAsync(IGameContext game)
		{
			string gameVersion = "monogame";
			string saveGameVersion = this.worldManager.World.GameVersion;
			var mustSendChangelog = false;
			
			if (String.CompareOrdinal(gameVersion, saveGameVersion) != 0)
			{
				ProtectedWorldState state = worldManager.World.ProtectedWorldData.Value;
				state.GameVersion = gameVersion;
				worldManager.World.ProtectedWorldData.Value = state;
				mustSendChangelog = true;
			}

			if (mustSendChangelog)
			{
				WorldProfileData metaProfile = worldManager.World.Profiles.FirstOrDefault(x => x.NarrativeId == "meta");

				if (metaProfile.NarrativeId != "meta")
				{
					metaProfile.InstanceId = worldManager.GetNextObjectId();
					metaProfile.NarrativeId = "meta";
					metaProfile.Gender = Gender.Unknown;
					metaProfile.IsSocialPrivate = true;
					metaProfile.ChatName = "sociallydistant";
					metaProfile.ChatName = "Socially Distant";
					worldManager.World.Profiles.Add(metaProfile);
				}

				var message = new WorldMailData()
				{
					InstanceId = worldManager.GetNextObjectId(),
					From = metaProfile.InstanceId,
					To = socialService.PlayerProfile.ProfileId,
					Subject = "Socially Distant Update",
					ThreadId = worldManager.GetNextObjectId(),
					Document = new[]
					{
						new DocumentElement
						{
							ElementType = DocumentElementType.Text,
							Data = "A new version of Socially Distant has been installed since you last played."
						},
						new DocumentElement
						{
							ElementType = DocumentElementType.Text,
							Data = $"You are now on Socially Distant {gameVersion}. Here's what's changed."
						},
						new DocumentElement
						{
							ElementType = DocumentElementType.Text,
							Data = changelogMarkupAsset.Markup
						},
					}
				};
				
				worldManager.World.Emails.Add(message);
			}
			
			return Task.CompletedTask;
		}

		public static MailManager? Instance => singleton.Instance;
	}
}