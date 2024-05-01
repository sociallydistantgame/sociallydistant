using UI.Shell.InfoPanel;
using UnityExtensions;

namespace UI.Websites.SocialMedia
{
	public class SocialPostViewsHolder : AutoSizedItemsViewsHolder
	{
		private SocialPostController view;
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out view);
			base.CollectViews();
		}

		public void SetData(SocialPostModel model)
		{
			view.SetData(model);
		}
	}
}