using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.TheFallenGames.OSA.Core.SubComponents
{
	public class GridViewsHolderFinder<TParams, TCellViewsHolder> : ViewsHolderFinder
		where TParams : GridParams
		where TCellViewsHolder : CellViewsHolder, new()
	{
		protected GridAdapter<TParams, TCellViewsHolder> GridAdapter { get { return _GridAdapter; } }

		readonly GridAdapter<TParams, TCellViewsHolder> _GridAdapter;


		public GridViewsHolderFinder(GridAdapter<TParams, TCellViewsHolder> gridAdapter) : base(gridAdapter)
		{
			_GridAdapter = gridAdapter;
		}


		protected override AbstractViewsHolder GetViewsHolderFromRoot(RectTransform root)
		{
			return _GridAdapter.GetCellViewsHolderIfVisible(root);
		}

		// TODO may also add options to navigate transversally to the OSA's direction
	}
}
