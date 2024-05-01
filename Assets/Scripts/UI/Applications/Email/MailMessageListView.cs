#nullable enable
using System.Collections.Generic;
using System.Linq;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Social;
using System;

namespace UI.Applications.Email
{
	public sealed class MailMessageListView : OSA<BaseParamsWithPrefab, MailMessageListViewsHolder>
	{
		private SimpleDataHelper<IMailMessage> messages;

		public event Action<IMailMessage>? ItemSelected;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.messages = new SimpleDataHelper<IMailMessage>(this);
		}

		public void SetItems(IEnumerable<IMailMessage> source)
		{
			if (!IsInitialized)
				this.Init();
			
			this.messages.ResetItems(source.ToList());
		}
		
		/// <inheritdoc />
		protected override MailMessageListViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new MailMessageListViewsHolder();

			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(MailMessageListViewsHolder newOrRecycled)
		{
			IMailMessage message = messages[newOrRecycled.ItemIndex];

			newOrRecycled.Callback = OnItemSelected;
			newOrRecycled.UpdateViews(message);
			
			ScheduleComputeVisibilityTwinPass();
		}

		private void OnItemSelected(IMailMessage message)
		{
			this.ItemSelected?.Invoke(message);
		}
	}
}