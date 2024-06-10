#nullable enable
using UnityEngine;

namespace UI.ScrollViews
{
	public abstract class GridCellModel
	{
		private readonly int itemIndex;

		protected readonly RectTransform root;
		
		public int ItemIndex => itemIndex;
		
		public GridCellModel(int itemIndex)
		{
			this.itemIndex = itemIndex;
		}

		public virtual void CollectViews()
		{
			
		}
	}
}