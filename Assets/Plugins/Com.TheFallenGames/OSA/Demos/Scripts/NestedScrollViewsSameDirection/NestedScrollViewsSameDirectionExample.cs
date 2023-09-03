using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;

namespace Com.TheFallenGames.OSA.Demos.NestedScrollViewsSameDirection
{
	/// <summary>
	/// Example demonstrating how you can have nested scrollviews that have the same scrolling direction (vertical/horizontal).
	/// Basically, when you scroll down inside a child OSA and reach the end, the drag/scroll event is forwarded to the
	/// next parent that handles the said event. Look in inspector for which properties were modified.
	/// </summary>
    public class NestedScrollViewsSameDirectionExample : OSA<BaseParamsWithPrefab, ChildAdapterViewsHolder>
	{
		public SimpleDataHelper<ChildAdapterModel> Data { get; private set; }

		// Because we're not using lazylist, we limit this to relatively small number (under 10k),
		// but more importantly, this example requires you to manually scroll through the end of the children grids, 
		// so having a smaller item count is more appropriate in this case.
		const int MAX_ITEMS_ALLOWED_IN_CHILD = 100;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<ChildAdapterModel>(this);

			base.Start();
		}

		/// <inheritdoc/>
		protected override ChildAdapterViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new ChildAdapterViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			// Keeping the same time scale in children as in the parent
			instance.childGrid.Parameters.UseUnscaledTime = _Params.UseUnscaledTime;

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(ChildAdapterViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			ChildAdapterModel childAdapterModel = Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(childAdapterModel);

			if (childAdapterModel.items == null) // first time updating the child scroll view => download initial items
				CreteRandomItemsForChildScrollView(newOrRecycled, childAdapterModel, UnityEngine.Random.Range(1, MAX_ITEMS_ALLOWED_IN_CHILD));
			else
				newOrRecycled.childGrid.SetItemsAndUpdate(childAdapterModel.items); // already having the items => update directly
		}

		/// <inheritdoc/>
		protected override void OnBeforeRecycleOrDisableViewsHolder(ChildAdapterViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			// Resetting the scroll position to be at start
			// 0 = left for horizontal ScrollViews (using Unity's ScrollRect convention)
			if (inRecycleBinOrVisible.childGrid != null)
				try { inRecycleBinOrVisible.childGrid.SetNormalizedPosition(0f); } catch { }

			inRecycleBinOrVisible.childGrid.ClearItemsAndUpdate();
			inRecycleBinOrVisible.childGrid.StopMovement();

			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
		}
		#endregion

		void CreteRandomItemsForChildScrollView(ChildAdapterViewsHolder viewsHolder, ChildAdapterModel childScrollViewModel, int count)
		{
			int nextFreeId = viewsHolder.childGrid.GetItemsCount(); // the "id" (used only to show it in TitleText) for the first created item is the current items count;
			var list = new List<MyCellModel>(count);
			for (int i = 0; i < count; i++)
			{
				var cellModel = new MyCellModel()
				{
					title = nextFreeId + "",
					color = DemosUtil.GetRandomColor(false)
				};
				++nextFreeId;
				list.Add(cellModel);
			}

			// The childScrollViewModel keeps track of items, because the 
			// child script is not a "persistent storage" - it's being constantly recycled
			childScrollViewModel.items = new List<MyCellModel>(list);
			viewsHolder.childGrid.SetItemsAndUpdate(list);
		}

		MyCellModel CreateNewChildModelWithIndex(int itemIndex)
		{
			return new MyCellModel()
			{
				title = itemIndex + "",
			};
		}
	}


	public class ChildAdapterModel
	{
		public string title;
		// The models of the child items are stored here, because the whole child view tree is being constantly re-used
		public List<MyCellModel> items = null; // keeping it null so we can know when to download the models for the first time
	}


	public class ChildAdapterViewsHolder : BaseItemViewsHolder
	{
		public Text titleText;
		public ChildGridSimpleExample childGrid;


		/// <inheritdoc/>
		protected override void OnRootCreated(int itemIndex, bool activateRootGameObject = true, bool callCollectViews = true)
		{
			base.OnRootCreated(itemIndex, activateRootGameObject, callCollectViews);

			// Initialization of child OSA is done manually to keep full control over its lifecycle
			childGrid.Init();
		}

		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			root.GetComponentAtPath("TitleText", out titleText);
			root.GetComponentAtPath("ChildGrid", out childGrid);
		}

		public void UpdateViews(ChildAdapterModel model)
		{
			titleText.text = model.title;
		}
	}
}