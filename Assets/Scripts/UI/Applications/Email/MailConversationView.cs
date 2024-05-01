#nullable enable
using System.Collections.Generic;
using System.Linq;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Social;

namespace UI.Applications.Email
{
	public sealed class MailConversationView : OSA<BaseParamsWithPrefab, MailConversationItemViewsHolder>
	{
		private SimpleDataHelper<IMailMessage> messages;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			messages = new SimpleDataHelper<IMailMessage>(this);
		}

		public void ViewMessage(IMailMessage? message)
		{
			if (!IsInitialized)
				this.Init();

			if (message == null)
			{
				this.messages.ResetItems(new List<IMailMessage>());
			}
			else if (message.Thread == null)
			{
				messages.ResetItems(new List<IMailMessage>() { message });
			}
			else
			{
				messages.ResetItems(message.Thread.GetMessagesInThread().ToList());
			}
		}
		
		/// <inheritdoc />
		protected override MailConversationItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new MailConversationItemViewsHolder();

			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex, false);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(MailConversationItemViewsHolder newOrRecycled)
		{
			IMailMessage message = messages[newOrRecycled.ItemIndex];
			
			newOrRecycled.UpdateViews(message);

			ScheduleComputeVisibilityTwinPass();
		}
	}
}