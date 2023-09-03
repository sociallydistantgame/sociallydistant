using System.Collections.Generic;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;
using System;

namespace Com.TheFallenGames.OSA.Demos.SimpleNestedScrollViews
{
	/// <summary>
	/// We're using a PageView structure as the parent OSA because it's a recurring concept across apps in general, so it's another
	/// good example where we can play with nested ScrollViews.
	/// The base code for this script was copied from <see cref="Com.TheFallenGames.OSA.Demos.PageViewExample"/> demo, since it's similar to it.
	/// For more info on that, including the Snapper8 and DiscreteScrollbar (which are handy for a PageView, but not necessary to understand 
	/// Nested ScrollViews), check tht example.
	/// </summary>
	public class SimpleNestedScrollViewsExample : OSA<BaseParamsWithPrefab, SimplePageVH>
	{
		/// <summary>
		/// Does what it says. You need to assign this to provide the page model for a given index.
		/// This will only be called once for each index, if and when the associated page will need to be displayed.
		/// </summary>
		public Func<int, PageModel> CreatePageModelAtIndexFunc { get; set; }
		
		/// <summary>
		/// Using a lazy data creation, because PageViews may have large amounts of data and we don't need to create it at once.
		/// If you want to avoid the added complexity of <see cref="LazyDataHelper{T}"/>, use a <see cref="SimpleDataHelper{T}"/> instead,
		/// but note that the models for all of pages and the models of their items will be created at once, and so the UI might freeze 
		/// for a while if you'll add, for example, 10 pages with 50.000 items each, leading to 10x50.000= 500.000 iterations. 
		/// </summary>
		public LazyDataHelper<PageModel> Data { get; private set; }


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new LazyDataHelper<PageModel>(this, CreatePageModelAtIndex);

			base.Start();

			GetComponentInChildren<DiscreteScrollbar>().getItemsCountFunc = () => Data.Count;
		}

		/// <inheritdoc/>
		protected override SimplePageVH CreateViewsHolder(int itemIndex)
		{
			var instance = new SimplePageVH();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			// Keeping the same time scale in children as in the parent
			instance.ChildListOSA.Parameters.UseUnscaledTime = _Params.UseUnscaledTime;

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(SimplePageVH newOrRecycled)
		{
			// Initialize the views from the associated model
			var model = Data.GetOrCreate(newOrRecycled.ItemIndex);
			newOrRecycled.UpdateViews(model);
		}

		protected override void OnBeforeRecycleOrDisableViewsHolder(SimplePageVH inRecycleBinOrVisible, int newItemIndex)
		{
			// Before a page is recycled, we reset its scroll position to be at start and cancel any scroll inertia/animations
			// so it'll have a clean state to display another page model
			var childOSA = inRecycleBinOrVisible.ChildListOSA;
			if (childOSA != null)
			{
				// 1f = top for vertical ScrollViews (using Unity's ScrollRect convention)
				try { childOSA.SetNormalizedPosition(1f); } catch { }
				childOSA.StopMovement();
				childOSA.CancelAllAnimations();
			}

			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
		}
		#endregion

		/// <summary>Used by the <see cref="DiscreteScrollbar"/>'s event</summary>
		public void ScrollToPage(int index)
		{
			SmoothScrollTo(index, .7f, .5f, .5f);
		}

		PageModel CreatePageModelAtIndex(int index)
		{
			return CreatePageModelAtIndexFunc(index);
		}
	}


	public class PageModel
	{
		public string Title { get; set; }
		public List<ChildListItemModel> Items { get; set; }
	}


	public class SimplePageVH : BaseItemViewsHolder
	{
		public SimpleNestedScrollViewsExampleChildList ChildListOSA { get; private set; }

		Text _TitleText;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			root.GetComponentAtPath("TitleText", out _TitleText);
			ChildListOSA = root.GetComponent<SimpleNestedScrollViewsExampleChildList>();
		}

		public void UpdateViews(PageModel model)
		{
			_TitleText.text = model.Title;

			// The child List OSA's Start() might've not been called yet, in which case it's not initialized,
			// so we're doing it manually.
			if (!ChildListOSA.IsInitialized)
				ChildListOSA.Init();

			ChildListOSA.Data.ResetItems(model.Items);
		}
	}
}
