using System;
using Social;
using UI.Shell.InfoPanel;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public class GuildViewsHolder : AutoSizedItemsViewsHolder
	{
		private GuildIconView view;
		
		public Action<IGuild>? Callback { get; set; }
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out view);

			view.Callback = this.OnSelected;
			
			base.CollectViews();
		}

		public void UpdateModel(GuildItemModel model)
		{
			view.UpdateModel(model);
		}

		private void OnSelected(IGuild guild)
		{
			Callback?.Invoke(guild);
		}

		/// <inheritdoc />
		public GuildViewsHolder(int itemIndex) : base(itemIndex)
		{ }
	}
}