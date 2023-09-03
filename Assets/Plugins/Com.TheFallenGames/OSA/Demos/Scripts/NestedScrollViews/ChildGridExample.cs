using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;

namespace Com.TheFallenGames.OSA.Demos.NestedScrollViews
{
    public class ChildGridExample : GridAdapter<MyGridParams, MyCellViewsHolder>
	{
		List<MyCellModel> _Data = new List<MyCellModel>(0);


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			// Start was overridden so that Init is not called automatically (see base.Start()), because Init is called manually externally,
			// which is done by whoever is responsible for creating this adapter (in our case, NestedScrollViewExample)
		}

		/// <inheritdoc/>
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int cellsCount, int indexIfAppendingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			base.ChangeItemsCount(changeMode, cellsCount, indexIfAppendingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);

			// Showing a loading indicator when there are no items
			SetWaitingForData(GetItemsCount() == 0);
		}

		/// <inheritdoc/>
		protected override void UpdateCellViewsHolder(MyCellViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			MyCellModel model = _Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);
		}
		#endregion

		public void SetWaitingForData(bool waiting)
		{
			_Params.loadingTextOverlay.gameObject.SetActive(waiting);
		}

		public bool IsWaitingForData()
		{
			return _Params.loadingTextOverlay.gameObject.activeSelf;
		}

		public void SetItemsAndUpdate(List<MyCellModel> items)
		{
			_Data = new List<MyCellModel>(items);
			NotifyAdapter();
		}

		public void AddItemsAndUpdate(List<MyCellModel> items)
		{
			_Data.AddRange(items);
			NotifyAdapter();
		}

		public void RemoveItemsAndUpdate(int count)
		{
			_Data.RemoveRange(_Data.Count - count, count);
			NotifyAdapter();
		}

		public void ClearItemsAndUpdate()
		{
			_Data.Clear();
			NotifyAdapter();
		}

		void NotifyAdapter()
		{ ResetItems(_Data.Count, false, true /*keepVelocity*/); }
	}


	[Serializable] // serializable, so it can be shown in inspector
	public class MyGridParams : GridParams
	{
		public Text loadingTextOverlay;
	}


	public class MyCellModel
	{
		public string title;
		public Sprite image;
	}


	public class MyCellViewsHolder : CellViewsHolder
	{
		public Text titleText;
		public Image image;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			views.GetComponentAtPath("TitleText", out titleText);
			views.GetComponentAtPath("BackgroundImage", out image);
		}

		public void UpdateViews(MyCellModel model)
		{
			titleText.text = model.title;
			image.sprite = model.image;
		}
	}
}