using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.TheFallenGames.OSA.Core.SubComponents.List
{
	public class ListViewsHolderFinder : ViewsHolderFinder
	{
		public ListViewsHolderFinder(IOSA adapter) : base(adapter)
		{
		}

		protected override AbstractViewsHolder GetViewsHolderFromRoot(RectTransform root)
		{
			return Adapter.GetBaseItemViewsHolderIfVisible(root);
		}
	}
}
