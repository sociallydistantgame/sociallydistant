using System.Collections.Generic;
using UI.ScrollViews;

namespace UI.Websites.SocialMedia
{
	public class SocialPostListView : ScrollViewController<SocialPostViewsHolder>
	{
		private ScrollViewItemList<SocialPostModel> posts;

		/// <inheritdoc />
		protected override void Awake()
		{
			posts = new ScrollViewItemList<SocialPostModel>(this);
			base.Awake();
		}

		public void SetItems(IList<SocialPostModel> items)
		{
			this.posts.SetItems(items);
		}
		
		/// <inheritdoc />
		protected override SocialPostViewsHolder CreateModel(int itemIndex)
		{
			var vh = new SocialPostViewsHolder(itemIndex);

			//vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(SocialPostViewsHolder newOrRecycled)
		{
			SocialPostModel post = posts[newOrRecycled.ItemIndex];

			newOrRecycled.SetData(post);
		}
	}
}