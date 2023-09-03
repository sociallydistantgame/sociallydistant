using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.TheFallenGames.OSA.Core.SubComponents
{
	public abstract class NavigationManager<TParams, TItemViewsHolder> : BaseNavigationManager
		where TParams : BaseParams
		where TItemViewsHolder : BaseItemViewsHolder
	{
		protected new OSA<TParams, TItemViewsHolder> Adapter { get { return base.Adapter as OSA<TParams, TItemViewsHolder>; } }


		public NavigationManager(OSA<TParams, TItemViewsHolder> adapter) :base(adapter)
		{
		}
	}
}
