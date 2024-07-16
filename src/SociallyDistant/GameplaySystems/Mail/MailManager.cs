#nullable enable

using System.Net.Mime;
using Microsoft.Xna.Framework;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.WorldData;
using SociallyDistant.GameplaySystems.Social;

namespace SociallyDistant.GameplaySystems.Mail
{
	public class MailManager : GameComponent
	{
		private readonly SociallyDistantGame gameManager;
		private          MarkupAsset         changelogMarkupAsset = null!;

		private static readonly Singleton<MailManager> singleton = new();
		
		private readonly Dictionary<ObjectId, MailThread> threads = new Dictionary<ObjectId, MailThread>();
		private readonly Dictionary<ObjectId, MailMessage> messages = new Dictionary<ObjectId, MailMessage>();
		
		private ISocialService SocialService => gameManager.SocialService;
		private WorldManager WorldManager => WorldManager.Instance!;
		
		internal MailManager(SociallyDistantGame game) : base(game)
		{
			singleton.SetInstance(this);
			this.gameManager = game;
		}

		public override void Initialize()
		{
			base.Initialize();
			InstallEvents();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (!disposing)
				return;
			
			UninstallEvents();
			
			singleton.SetInstance(null);
		}

		private void InstallEvents()
		{
			WorldManager.Callbacks.AddCreateCallback<WorldMailData>(OnCreateMail);
			WorldManager.Callbacks.AddModifyCallback<WorldMailData>(OnModifyMail);
			WorldManager.Callbacks.AddDeleteCallback<WorldMailData>(OnDeleteMail);
		}

		private void UninstallEvents()
		{
			WorldManager.Callbacks.RemoveCreateCallback<WorldMailData>(OnCreateMail);
			WorldManager.Callbacks.RemoveModifyCallback<WorldMailData>(OnModifyMail);
			WorldManager.Callbacks.RemoveDeleteCallback<WorldMailData>(OnDeleteMail);
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
				this.From = this.mailManager.SocialService.GetProfileById(mailData.From);
				this.To = this.mailManager.SocialService.GetProfileById(mailData.To);

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

		public static MailManager? Instance => singleton.Instance;
	}
}