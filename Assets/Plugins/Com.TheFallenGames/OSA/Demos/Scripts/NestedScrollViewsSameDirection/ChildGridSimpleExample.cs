using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;

namespace Com.TheFallenGames.OSA.Demos.NestedScrollViewsSameDirection
{
    public class ChildGridSimpleExample : GridAdapter<GridParams, MyCellViewsHolder>
	{
		List<MyCellModel> _Data = new List<MyCellModel>(0);


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			// Start was overridden so that Init is not called automatically (see base.Start()), because Init is called manually externally,
			// which is done by whoever is responsible for creating this adapter (in our case, NestedScrollViewsSameDirectionExample)
		}

		/// <inheritdoc/>
		protected override void UpdateCellViewsHolder(MyCellViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			MyCellModel model = _Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);
		}
		#endregion

		public void SetItemsAndUpdate(List<MyCellModel> items)
		{
			_Data = new List<MyCellModel>(items);
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


	public class MyCellModel
	{
		public string title;
		public Color color;
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
			image.color = model.color;
		}
	}
}