using System;
using System.Collections.Generic;
using Social;
using UI.ScrollViews;

namespace UI.Applications.Chat
{
	public class GuildListView : ScrollViewController<GuildViewsHolder>
	{
		private ScrollViewItemList<GuildItemModel> guildItems;

		public event Action<IGuild>? GuildSelected; 
		
		/// <inheritdoc />
		protected override void Awake()
		{
			guildItems = new ScrollViewItemList<GuildItemModel>(this);
			base.Awake();
		}

		public void SetItems(IList<GuildItemModel> items)
		{
			this.guildItems.SetItems(items);
		}

		/// <inheritdoc />
		protected override GuildViewsHolder CreateModel(int itemIndex)
		{
			var vh = new GuildViewsHolder();
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(GuildViewsHolder newOrRecycled)
		{
			GuildItemModel model = guildItems[newOrRecycled.ItemIndex];

			newOrRecycled.UpdateModel(model);
			
			newOrRecycled.Callback = OnGuildSelected;
		}

		private void OnGuildSelected(IGuild guild)
		{
			this.GuildSelected?.Invoke(guild);
		}
	}
}