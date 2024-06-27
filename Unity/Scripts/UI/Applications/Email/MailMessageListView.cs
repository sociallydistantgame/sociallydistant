#nullable enable
using System.Collections.Generic;
using System.Linq;
using Social;
using System;
using UI.ScrollViews;

namespace UI.Applications.Email
{
	public sealed class MailMessageListView : ScrollViewController<MailMessageListViewsHolder>
	{
		private ScrollViewItemList<IMailMessage> messages;

		public event Action<IMailMessage>? ItemSelected;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.messages = new ScrollViewItemList<IMailMessage>(this);
		}

		public void SetItems(IEnumerable<IMailMessage> source)
		{
			this.messages.SetItems(source.ToList());
		}
		
		/// <inheritdoc />
		protected override MailMessageListViewsHolder CreateModel(int itemIndex)
		{
			var vh = new MailMessageListViewsHolder();
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(MailMessageListViewsHolder newOrRecycled)
		{
			IMailMessage message = messages[newOrRecycled.ItemIndex];

			newOrRecycled.Callback = OnItemSelected;
			newOrRecycled.UpdateViews(message);
		}

		private void OnItemSelected(IMailMessage message)
		{
			this.ItemSelected?.Invoke(message);
		}
	}
}