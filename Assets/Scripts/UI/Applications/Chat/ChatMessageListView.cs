using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

namespace UI.Applications.Chat
{
	public class ChatMessageListView : OSA<BaseParamsWithPrefab, ChatMessageViewsHolder>
	{
		private SimpleDataHelper<ChatMessageModel> messages;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			messages = new SimpleDataHelper<ChatMessageModel>(this);
		}

		public void SetItems(IList<ChatMessageModel> messageList)
		{
			if (!IsInitialized)
				Init();
			
			messages.ResetItems(messageList);
		}

		/// <inheritdoc />
		protected override ChatMessageViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new ChatMessageViewsHolder();

			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(ChatMessageViewsHolder newOrRecycled)
		{
			ChatMessageModel model = messages[newOrRecycled.ItemIndex];

			newOrRecycled.UpdateView(model);
			
			ScheduleComputeVisibilityTwinPass(true);
		}
	}
}