using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.CustomAdapters.GridView.Specialized.GridWithCategories;

namespace Com.TheFallenGames.OSA.Demos.GridWithCategories
{
	/// <summary>
	/// <para>A special case of grid allowing convenient headers of varying size between virtual categories. They are virtual because 
	/// OSA doesn't know about them. They're simply game objects inside your own provided cell group prefab and you disable/enable them on demand.</para>
	/// 
	/// <para>This example combines the concept of Grid and the ContentSizeFitter to achieve different sizes for cell groups (heights of rows in this case).</para>
	/// 
	/// <para>DataHelpers are of no use here, because the list of category models needs to be transformed into a flattened list of cell models, which also 
	/// have some dummy cells inserted here and there (more info can be found below and in <see cref="GridWithCategoriesDemoDataUtil"/>)</para>
	/// </summary>
	public class GridWithCategoriesExample : GridAdapter<MyGridParams, MyCellViewsHolder>
	{
		/// <summary>Using a regular list, since items aren't added/removed through the DataHelper anyway</summary>
		List<CellModel> Data;

		/// <summary>This is the original input data. It's used to regenerate the Data that's used with OSA whenever needed</summary>
		List<CategoryModel> _Categories;

		/// <summary> Used in PostRebuildLayoutDueToScrollViewSizeChange to detect if the list of cells needs to be regenerated </summary>
		int _LastKnownNumberOfCellsPerGroup;


		#region GridAdapter implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new List<CellModel>(0);

			base.Start();
		}

		/// <seealso cref="GridAdapter{TParams, TCellVH}.Refresh(bool, bool)"/>
		public override void Refresh(bool contentPanelEndEdgeStationary = false /*ignored*/, bool keepVelocity = false)
		{
			_CellsCount = Data.Count;
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

			// This is a Row prefab. We're updating it as a header if it's the special kind of row (the empty row separating our categories)
			if (newOrRecycled.NumActiveCells > 0)
			{
				var firstCellVH = newOrRecycled.ContainingCellViewsHolders[0];
				int modelIndex = firstCellVH.ItemIndex;
				var model = Data[modelIndex];
				var newOrRecycledCasted = newOrRecycled as MyCellGroupViewsHolder;
				if (model.Type == CellType.IN_ROW_SEPARATING_CATEGORIES)
					newOrRecycledCasted.ShowHeader((model.ParentCategory as CategoryModel).name);
				else
					newOrRecycledCasted.ClearHeader();
			}

			// Constantly triggering a twin pass after the current normal pass, so the CSFs will be updated
			// This can be optimized by keeping track of what items already had their size calculated, but for our purposes it's enough
			ScheduleComputeVisibilityTwinPass();
		}

		/// <summary> Called when a cell becomes visible </summary>
		/// <param name="viewsHolder"> use viewsHolder.ItemIndex to find your corresponding model and feed data into its views</param>
		/// <see cref="GridAdapter{TParams, TCellVH}.UpdateCellViewsHolder(TCellVH)"/>
		protected override void UpdateCellViewsHolder(MyCellViewsHolder viewsHolder)
		{
			var model = Data[viewsHolder.ItemIndex];
			
			viewsHolder.UpdateViews(model, _Params);
		}

		/// <inheritdoc/>
		protected override void PostRebuildLayoutDueToScrollViewSizeChange()
		{
			base.PostRebuildLayoutDueToScrollViewSizeChange();

			// Regenerating the list of cells that'll be passed to OSA if the number of cells per group (row, in this case) has changed
			if (_LastKnownNumberOfCellsPerGroup != _Params.CurrentUsedNumCellsPerGroup)
				ResetData(_Categories);
		}
		#endregion

		/// <summary>Data can only be modified using this method, because and intermediary conversion step is needed before being able to actually display it</summary>
		/// <param name="categories"></param>
		public void ResetData(List<CategoryModel> categories)
		{
			_Categories = categories;
			var cellsPerRow = _Params.CurrentUsedNumCellsPerGroup;
			List<CellModel> cellsList;
			GridWithCategoriesUtil.ConvertCategoriesToListOfItemModels(cellsPerRow, _Categories, out cellsList);
			_LastKnownNumberOfCellsPerGroup = cellsPerRow;
			Data = cellsList;

			// Since we don't use a DataHelper to notify OSA for us, we do it manually
			ResetItems(Data.Count, false, true); // the last 2 params are not important. Can be omitted if you want
		}
	}


	/// <inheritdoc/>
	[Serializable]
	public class MyGridParams : GridParams
	{
		// We use a custom prefab of the group of cells (row of cells in this case)
		[SerializeField]
		GameObject _CellGroupPrefab = null;

		// Space between our virtual categories
		[SerializeField]
		float _HeaderSize = 50f;

		public float HeaderSize { get { return _HeaderSize; } }


		protected override GameObject CreateCellGroupPrefabGameObject()
		{
			// Already created
			return _CellGroupPrefab;
		}
	}


	public class CategoryModel : ICategoryModel
	{
		public string name;
		public List<CellModel> items;

		public int Count { get { return items.Count; } }
		public ICellModel this[int index] { get { return items[index]; } }
	}


	public class CellModel : ICellModel
	{
		public ICategoryModel ParentCategory { get; set; }
		public int Id { get; set; }
		public CellType Type { get; set; }
		public readonly Color color = DemosUtil.GetRandomColor(true);
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
		}

		public void UpdateViews(CellModel model, MyGridParams parameters)
		{
			switch (model.Type)
			{
				case CellType.VALID:
					SetVisible(true);

					rootLayoutElement.ignoreLayout = false;
					_Title.text = "#" + ItemIndex + " [id:" + model.Id + "]";
					_Background.color = model.color;
					break;

				// For this type of cell, we disable it to Make room for others and respect the Group's Gravity/Alignment settings
				case CellType.FOR_ROW_COMPLETION:
				// Element inside an empty (separator) row
				case CellType.IN_ROW_SEPARATING_CATEGORIES:
					SetVisible(false);
					rootLayoutElement.ignoreLayout = true;
					break;
			}
		}

		void SetVisible(bool enabled)
		{
			_Background.enabled = enabled;
			_Title.enabled = enabled;
		}
	}


	/// <summary>
	/// A custom cell group views holder can be implemented, if additional functionality is needed, like 
	/// in our case - we need to have a header object in each row and show/hide it based on whether that row is a separator
	/// or not
	/// </summary>
	public class MyCellGroupViewsHolder : CellGroupViewsHolder<MyCellViewsHolder>
	{
		public UnityEngine.UI.ContentSizeFitter contentSizeFitterComponent;

		Transform _HeaderPanel;
		Text _HeaderText;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			// Since the group views holder is created at runtime internally, we also need to add the CSF by code
			contentSizeFitterComponent = root.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
			contentSizeFitterComponent.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

			// Keeping the CSF always enabled is easier to manage. We'll trigger a Twin pass very frequently, anyway
			contentSizeFitterComponent.enabled = true;

			// Each group has a header that's initially hidden
			root.GetComponentAtPath("HeaderPanel", out _HeaderPanel);
			_HeaderPanel.GetComponentAtPath("HeaderPanelFit/HeaderText", out _HeaderText);
		}

		public void ShowHeader(string text)
		{
			_HeaderPanel.gameObject.SetActive(true);
			_HeaderText.text = text;

		}

		public void ClearHeader()
		{
			_HeaderPanel.gameObject.SetActive(false);
		}
	}
}
