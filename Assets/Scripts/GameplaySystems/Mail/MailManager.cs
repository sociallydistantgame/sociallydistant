#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Scripting;
using Core.WorldData;
using Core.WorldData.Data;
using GamePlatform;
using GameplaySystems.Social;
using Modules;
using Social;
using UnityEngine;
using UnityEngine.Analytics;
using UnityExtensions;

namespace GameplaySystems.Mail
{
	public class MailManager : 
		MonoBehaviour,
		IHookListener
	{
		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		[SerializeField]
		private WorldManagerHolder worldManager = null!;

		[SerializeField]
		private SocialServiceHolder socialServiceHolder = null!;

		[SerializeField]
		private MarkupAsset changelogMarkupAsset = null!;
		
		private readonly Dictionary<ObjectId, MailThread> threads = new Dictionary<ObjectId, MailThread>();
		private readonly Dictionary<ObjectId, MailMessage> messages = new Dictionary<ObjectId, MailMessage>();
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(MailManager));
		}

		private void Start()
		{
			if (gameManager.Value != null)
				gameManager.Value.ScriptSystem.RegisterHookListener(CommonScriptHooks.AfterWorldStateUpdate, this);
			
			InstallEvents();
		}

		private void OnDestroy()
		{
			UninstallEvents();
			
			if (gameManager.Value != null)
				gameManager.Value.ScriptSystem.UnregisterHookListener(CommonScriptHooks.AfterWorldStateUpdate, this);
		}

		private void InstallEvents()
		{
			if (worldManager.Value != null)
			{
				worldManager.Value.Callbacks.AddCreateCallback<WorldMailData>(OnCreateMail);
				worldManager.Value.Callbacks.AddModifyCallback<WorldMailData>(OnModifyMail);
				worldManager.Value.Callbacks.AddDeleteCallback<WorldMailData>(OnDeleteMail);
				
			}
		}

		private void UninstallEvents()
		{
			if (worldManager.Value != null)
			{
				worldManager.Value.Callbacks.RemoveCreateCallback<WorldMailData>(OnCreateMail);
				worldManager.Value.Callbacks.RemoveModifyCallback<WorldMailData>(OnModifyMail);
				worldManager.Value.Callbacks.RemoveDeleteCallback<WorldMailData>(OnDeleteMail);
			}
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
				this.From = this.mailManager.socialServiceHolder.Value.GetProfileById(mailData.From);
				this.To = this.mailManager.socialServiceHolder.Value.GetProfileById(mailData.To);

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
				this.Body = mailData.Document.ToArray();
			}
		}

		/// <inheritdoc />
		public Task ReceiveHookAsync(IGameContext game)
		{
			if (this.socialServiceHolder.Value == null)
				return Task.CompletedTask;

			if (this.worldManager.Value == null)
				return Task.CompletedTask;
			
			string gameVersion = Application.version;
			string saveGameVersion = this.worldManager.Value.World.ProtectedWorldData.Value.GameVersion;
			var mustSendChangelog = false;
			
			if (String.CompareOrdinal(gameVersion, saveGameVersion) != 0)
			{
				ProtectedWorldState state = worldManager.Value.World.ProtectedWorldData.Value;
				state.GameVersion = gameVersion;
				worldManager.Value.World.ProtectedWorldData.Value = state;
				mustSendChangelog = true;
			}

			if (mustSendChangelog)
			{
				WorldProfileData metaProfile = worldManager.Value.World.Profiles.FirstOrDefault(x => x.NarrativeId == "meta");

				if (metaProfile.NarrativeId != "meta")
				{
					metaProfile.InstanceId = worldManager.Value.GetNextObjectId();
					metaProfile.NarrativeId = "meta";
					metaProfile.Gender = Gender.Unknown;
					metaProfile.IsSocialPrivate = true;
					metaProfile.ChatName = "sociallydistant";
					metaProfile.ChatName = "Socially Distant";
					worldManager.Value.World.Profiles.Add(metaProfile);
				}

				var message = new WorldMailData()
				{
					InstanceId = worldManager.Value.GetNextObjectId(),
					From = metaProfile.InstanceId,
					To = socialServiceHolder.Value.PlayerProfile.ProfileId,
					Subject = "Socially Distant Update",
					ThreadId = worldManager.Value.GetNextObjectId(),
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
				
				worldManager.Value.World.Emails.Add(message);
			}
			
			return Task.CompletedTask;
		}
	}
}