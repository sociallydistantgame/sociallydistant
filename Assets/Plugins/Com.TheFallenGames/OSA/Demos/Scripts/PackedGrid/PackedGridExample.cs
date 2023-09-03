using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;
using frame8.Logic.Misc.Other.Extensions;

namespace Com.TheFallenGames.OSA.Demos.GridDifferentItemSizes
{
    /// <summary>
    /// Advanced implementation demonstrating the usage of a <see cref="GridAdapter{TParams, TCellVH}"/> and a custom Views holder for the cell groups, <see cref="PackedGridLayoutGroup"/>,
	/// Which packs the cells in the smallest space possible instead of arranging them in a line. 
	/// The bigger the cell group in physical size, the more accurate the packing algorithm is.
    /// </summary>
    public class PackedGridExample : GridAdapter<MyGridParams, MyCellViewsHolder>
	{
		// Initialized outside
		public LazyDataHelper<BasicModel> LazyData { get; set; }


		#region GridAdapter implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			// Prevent initialization. It'll be done from outside
			//base.Start();
		}

		/// <seealso cref="GridAdapter{TParams, TCellVH}.Refresh(bool, bool)"/>
		public override void Refresh(bool contentPanelEndEdgeStationary = false /*ignored*/, bool keepVelocity = false)
		{
			_CellsCount = LazyData.Count;

			// Invalidate the last sizes so that they'll be re-calculated
			foreach (var model in LazyData.GetEnumerableForExistingItems())
				model.hasPendingFlexibleWidthChange = true;

			base.Refresh(false, keepVelocity);
		}

		/// <inheritdoc/>
		protected override CellGroupViewsHolder<MyCellViewsHolder> GetNewCellGroupViewsHolder()
		{
			// Create cell group holders of our custom type (which stores the CSF component)
			return new MyCellGroupViewsHolder();
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(CellGroupViewsHolder<MyCellViewsHolder> newOrRecycled)
		{
			base.UpdateViewsHolder(newOrRecycled);

			// Constantly triggering a twin pass after the current normal pass, so the CSFs will be updated
			ScheduleComputeVisibilityTwinPass();
		}

		/// <summary> Called when a cell becomes visible </summary>
		/// <param name="viewsHolder"> use viewsHolder.ItemIndex to find your corresponding model and feed data into its views</param>
		/// <see cref="GridAdapter{TParams, TCellVH}.UpdateCellViewsHolder(TCellVH)"/>
		protected override void UpdateCellViewsHolder(MyCellViewsHolder viewsHolder)
		{
			var model = LazyData.List[viewsHolder.ItemIndex];
			
			if (model.hasPendingFlexibleWidthChange)
				ComputeModelSize(model);

			viewsHolder.UpdateViews(model);
			viewsHolder.rootLayoutElement.preferredWidth = model.computedPreferredWidth;
			viewsHolder.rootLayoutElement.preferredHeight = model.ComputedPreferredHeight;
		}
		#endregion


		void ComputeModelSize(BasicModel model)
		{
			//model.computedFlexibleWidth = _Params.GetFlexibleWidthFromWeight(model.weight);
			model.computedPreferredWidth = _Params.GetCellWidthFor(model);
		}
	}


	/// <inheritdoc/>
	[Serializable]
	public class MyGridParams : GridParams
	{
		[Tooltip("If true, the layout will start with bigger children. The starting position is defined by the 'Child Alignment' property")]
		public bool biggerChildrenFirst = true;


		protected override LayoutGroup AddLayoutGroupToCellGroupPrefab(GameObject cellGroupGameObject)
		{
			return cellGroupGameObject.AddComponent<PackedGridLayoutGroup>();
		}

		protected override void InitOrReinitCellGroupPrefabLayoutGroup(LayoutGroup cellGroupGameObject)
		{
			base.InitOrReinitCellGroupPrefabLayoutGroup(cellGroupGameObject);

			var packedGridLG = cellGroupGameObject as PackedGridLayoutGroup;
			packedGridLG.ForcedSpacing = Grid.SpacingInGroup;
			packedGridLG.ChildrenControlSize = PackedGridLayoutGroup.AxisOrNone.Vertical; // grow vertically
			packedGridLG.BiggerChildrenFirst = biggerChildrenFirst;
		}

		public float GetCellWidthFor(BasicModel model)
		{
			return model.weight * DefaultItemSize;
		}
	}


	public class BasicModel
	{
		public int id;
		public readonly Color color = DemosUtil.GetRandomColor(true);
		public int weight;
		public float aspectRatio = 1f; // width over height

		public float computedPreferredWidth;
		public float ComputedPreferredHeight { get { return computedPreferredWidth / aspectRatio; } }
		public bool hasPendingFlexibleWidthChange = true;
	}


	/// <summary>All views holders used with GridAdapter should inherit from <see cref="CellViewsHolder"/></summary>
	public class MyCellViewsHolder : CellViewsHolder
	{
		Text _Title;
		Image _Background;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			_Title = views.Find("TitleText").GetComponent<Text>();
			_Background = views.GetComponent<Image>();

			rootLayoutElement.preferredWidth = 0f;
		}

		public void UpdateViews(BasicModel model)
		{
			_Title.text = "#" + ItemIndex + " [id:" + model.id + "]";
			_Background.color = model.color;
		}
	}


	public class MyCellGroupViewsHolder : CellGroupViewsHolder<MyCellViewsHolder>
	{
		public UnityEngine.UI.ContentSizeFitter contentSizeFitterComponent;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			// Since the group views holder is created at runtime internally, we also need to add the CSF by code
			contentSizeFitterComponent = root.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
			contentSizeFitterComponent.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

			// Keeping the CSF always enabled is easier to manage. We'll trigger a Twin pass very frequently, anyway
			contentSizeFitterComponent.enabled = true;
		}
	}
}
