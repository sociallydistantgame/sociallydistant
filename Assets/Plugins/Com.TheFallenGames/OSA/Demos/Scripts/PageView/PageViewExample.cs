using System;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;

namespace Com.TheFallenGames.OSA.Demos.PageView
{
    /// <summary>
	/// Demonstrating a Page View which also allows for transitioning to the next/prev page when the drag speed exceeds a certain value, 
	/// not only when the current page is more than half outside. This is mainly thanks to <see cref="Snapper8.minSpeedToAllowSnapToNext"/>
	/// </summary>
    public class PageViewExample : OSA<MyParams, PageViewsHolder>
	{
		public SimpleDataHelper<PageModel> Data { get; private set; }


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<PageModel>(this);

			base.Start();

			GetComponentInChildren<DiscreteScrollbar>().getItemsCountFunc = () => Data.Count;
		}

		/// <inheritdoc/>
		protected override PageViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new PageViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(PageViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			PageModel model = Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);
		}
		#endregion

		/// <summary>Used by the <see cref="DiscreteScrollbar"/>'s event</summary>
		public void ScrollToPage(int index)
		{
			SmoothScrollTo(index, .7f, .5f, .5f);
		}
	}

	[Serializable]
	public class MyParams : BaseParamsWithPrefab
	{
	}

	public class PageModel
	{
		public string title, body;
		public Sprite image;
	}


	public class PageViewsHolder : BaseItemViewsHolder
	{
		public Text titleText, bodyText;
		public Image image;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			root.GetComponentAtPath("TitlePanel/TitleText", out titleText);
			root.GetComponentAtPath("BodyPanel/BodyText", out bodyText);
			root.GetComponentAtPath("BackgroundMask/BackgroundImage", out image);
		}

		public void UpdateViews(PageModel model)
		{
			titleText.text = model.title;
			bodyText.text = model.body;
			image.sprite = model.image;
		}
	}
}
