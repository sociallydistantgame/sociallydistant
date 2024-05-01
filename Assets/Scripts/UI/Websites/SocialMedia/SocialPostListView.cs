using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

namespace UI.Websites.SocialMedia
{
	public class SocialPostListView : OSA<BaseParamsWithPrefab, SocialPostViewsHolder>
	{
		private SimpleDataHelper<SocialPostModel> posts;

		/// <inheritdoc />
		protected override void Awake()
		{
			Init();

			posts = new SimpleDataHelper<SocialPostModel>(this);
			base.Awake();
		}

		public void SetItems(IList<SocialPostModel> items)
		{
			this.posts.ResetItems(items);
		}
		
		/// <inheritdoc />
		protected override SocialPostViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new SocialPostViewsHolder();

			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(SocialPostViewsHolder newOrRecycled)
		{
			SocialPostModel post = posts[newOrRecycled.ItemIndex];

			newOrRecycled.SetData(post);

			ScheduleComputeVisibilityTwinPass();
		}
	}
}