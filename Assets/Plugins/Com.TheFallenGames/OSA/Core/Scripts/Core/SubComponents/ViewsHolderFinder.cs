using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.TheFallenGames.OSA.Core.SubComponents
{
	public abstract class ViewsHolderFinder
	{
		protected IOSA Adapter { get; private set; }
		protected BaseParams.NavigationParams NavParams { get { return Adapter.BaseParameters.Navigation; } }


		public ViewsHolderFinder(IOSA adapter)
		{
			Adapter = adapter;
		}

		public virtual AbstractViewsHolder GetViewsHolderFromSelectedGameObject(GameObject curSelected)
		{
			int currentDepth = 0;
			int maxDepth = NavParams.MaxSearchDepthForViewsHolder;
			var curTR = curSelected.transform;
			while (curTR && currentDepth <= maxDepth)
			{
				var vh = GetViewsHolderFromRoot(curTR as RectTransform);
				if (vh != null)
				{
					return vh;
				}

				curTR = curTR.parent;
				++currentDepth;
			}

			return null;
		}

		protected abstract AbstractViewsHolder GetViewsHolderFromRoot(RectTransform root);
	}
}
