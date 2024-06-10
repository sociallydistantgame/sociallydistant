#nullable enable
using UnityEngine;

namespace UI.ScrollViews
{
	public abstract class ScrollViewModel
	{
		private readonly int itemIndex;

		protected readonly RectTransform root;
		
		public int ItemIndex => itemIndex;

		public ScrollViewModel(int itemIndex)
		{
			this.itemIndex = itemIndex;
		}

		public virtual void CollectViews()
		{
			
		}
	}
}