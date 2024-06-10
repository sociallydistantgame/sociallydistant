using System.Collections.Generic;
using UI.ScrollViews;

namespace UI.Applications.Chat
{
	public class ChatMessageListView : ScrollViewController<ChatMessageViewsHolder>
	{
		private ScrollViewItemList<ChatMessageModel> messages;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			messages = new ScrollViewItemList<ChatMessageModel>(this);
		}

		public void SetItems(IList<ChatMessageModel> messageList)
		{
			messages.SetItems(messageList);
		}

		/// <inheritdoc />
		protected override ChatMessageViewsHolder CreateModel(int itemIndex)
		{
			var vh = new ChatMessageViewsHolder(itemIndex);

			//vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(ChatMessageViewsHolder newOrRecycled)
		{
			ChatMessageModel model = messages[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateView(model);
		}

		public void ScrollTo(int index, float offset)
		{
			
		}
	}
}