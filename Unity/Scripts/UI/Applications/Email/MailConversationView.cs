#nullable enable
using System.Collections.Generic;
using System.Linq;
using Social;
using UI.ScrollViews;

namespace UI.Applications.Email
{
	public sealed class MailConversationView : ScrollViewController<MailConversationItemViewsHolder>
	{
		private ScrollViewItemList<IMailMessage> messages;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			messages = new ScrollViewItemList<IMailMessage>(this);
		}

		public void ViewMessage(IMailMessage? message)
		{
			if (message == null)
			{
				this.messages.SetItems(new List<IMailMessage>());
			}
			else if (message.Thread == null)
			{
				messages.SetItems(new List<IMailMessage>() { message });
			}
			else
			{
				messages.SetItems(message.Thread.GetMessagesInThread().ToList());
			}
		}
		
		/// <inheritdoc />
		protected override MailConversationItemViewsHolder CreateModel(int itemIndex)
		{
			var vh = new MailConversationItemViewsHolder();
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(MailConversationItemViewsHolder newOrRecycled)
		{
			IMailMessage message = messages[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(message);
		}
	}
}