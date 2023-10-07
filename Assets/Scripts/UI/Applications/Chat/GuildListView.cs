using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Social;

namespace UI.Applications.Chat
{
	public class GuildListView : OSA<BaseParamsWithPrefab, GuildViewsHolder>
	{
		private SimpleDataHelper<GuildItemModel> guildItems;

		public event Action<IGuild>? GuildSelected; 
		
		/// <inheritdoc />
		protected override void Awake()
		{
			guildItems = new SimpleDataHelper<GuildItemModel>(this);
			base.Awake();
		}

		public void SetItems(IList<GuildItemModel> items)
		{
			if (!IsInitialized)
				this.Init();

			this.guildItems.ResetItems(items);
		}

		/// <inheritdoc />
		protected override GuildViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new GuildViewsHolder();
			
			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(GuildViewsHolder newOrRecycled)
		{
			GuildItemModel model = guildItems[newOrRecycled.ItemIndex];

			newOrRecycled.UpdateModel(model);
			
			newOrRecycled.Callback = OnGuildSelected;
			
			ScheduleComputeVisibilityTwinPass();
		}

		private void OnGuildSelected(IGuild guild)
		{
			this.GuildSelected?.Invoke(guild);
		}
	}
}