using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.TheFallenGames.OSA.Core.SubComponents
{
	public class GridNavigationManager<TParams, TCellViewsHolder> : NavigationManager<TParams, CellGroupViewsHolder<TCellViewsHolder>>
		where TParams : GridParams
		where TCellViewsHolder : CellViewsHolder, new()
	{
		protected GridAdapter<TParams, TCellViewsHolder> GridAdapter { get { return base.Adapter as GridAdapter<TParams, TCellViewsHolder>; } }


		public GridNavigationManager(GridAdapter<TParams, TCellViewsHolder> gridAdapter) : base(gridAdapter)
		{
		}

		protected override ViewsHolderFinder CreateViewsHolderFinder()
		{
			return new GridViewsHolderFinder<TParams, TCellViewsHolder>(GridAdapter);
		}

		// TODO may also add options to navigate transversally to the OSA's direction
	}
}
