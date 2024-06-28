#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Social;

namespace SociallyDistant.GameplaySystems.Social
{
	public sealed class NarrativeThreadController : INarrativeThread
	{
		private readonly List<IProfile> typers = new List<IProfile>();
		private readonly IWorldManager worldManager;
		private readonly IProfile player;
		private readonly ObjectId threadId;
		
		public NarrativeThreadController(IWorldManager worldManager, IProfile player, ObjectId threadId)
		{
			this.worldManager = worldManager;
			this.player = player;
			this.threadId = threadId;
		}

		/// <inheritdoc />
		public ObjectId ChannelId => threadId;

		/// <inheritdoc />
		public async Task Say(IProfile sender, string textMessage)
		{
			// Crude typing delay
			if (sender != player)
			{
				MarkTyping(sender);
				await Task.Delay(textMessage.Length * 120);
				MarkNotTyping(sender);
			}

			var messageData = new WorldMessageData
			{
				InstanceId = worldManager.GetNextObjectId(),
				ChannelId = threadId,
				Author = sender.ProfileId,
				Date = worldManager.World.GlobalWorldState.Value.Now,
				DocumentElements = new List<DocumentElement>
				{
					new DocumentElement
					{
						ElementType = DocumentElementType.Text,
						Data = textMessage
					}
				}
			};
			
			if (sender != player)
			{
				worldManager.World.Notifications.Add(new WorldNotificationData
				{
					InstanceId = worldManager.GetNextObjectId(),
					CorrelationId = messageData.InstanceId,
					GroupId = NotificationGroups.Chat
				});
			}
			
			worldManager.World.Messages.Add(messageData);
		}

		/// <inheritdoc />
		public async Task AttachMission(IProfile sender, string missionId)
		{
			if (sender == this.player)
				throw new InvalidOperationException("Players cannot attach missions.");

			MarkTyping(sender);
			await Task.Delay(1200);
			MarkNotTyping(sender);
			
			worldManager.World.Messages.Add(new WorldMessageData
			{
				InstanceId = worldManager.GetNextObjectId(),
				ChannelId = threadId,
				Author = sender.ProfileId,
				Date = worldManager.World.GlobalWorldState.Value.Now,
				DocumentElements = new List<DocumentElement>
				{
					new DocumentElement
					{
						ElementType = DocumentElementType.Mission,
						Data = missionId
					}
				}
			});
		}

		private void MarkTyping(IProfile profile)
		{
			typers.Add(profile);
			UpdateTypersInWorld();
		}

		private void MarkNotTyping(IProfile profile)
		{
			typers.Remove(profile);
			UpdateTypersInWorld();
		}

		private void UpdateTypersInWorld()
		{
			WorldChannelData channelData = worldManager.World.Channels.FirstOrDefault(x => x.InstanceId == this.ChannelId);
			if (channelData.InstanceId.IsInvalid)
				return;

			channelData.TypingUsers = typers.Select(x => x.ProfileId).ToArray();
			
			worldManager.World.Channels.Modify(channelData);
		}
	}
}