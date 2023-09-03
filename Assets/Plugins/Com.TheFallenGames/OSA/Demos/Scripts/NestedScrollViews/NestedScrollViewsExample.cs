using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;

namespace Com.TheFallenGames.OSA.Demos.NestedScrollViews
{
    public class NestedScrollViewsExample : OSA<MyParams, ChildAdapterViewsHolder>
	{
		public SimpleDataHelper<ChildAdapterModel> Data { get; private set; }

		WaitForSeconds _WaitForSecondsCached = new WaitForSeconds(.1f);
		const int MAX_ITEMS_ALLOWED_IN_CHILD = 10 * 1000; // Because we're not using lazylist, we limit this to relatively small number


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
			instance.addRemoveButtonWithInput.button.onClick.AddListener(() => OnAddRemoveClickedOnChild(instance));

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
				TryDownloadingSomeItemsForChildScrollView(newOrRecycled, childAdapterModel, UnityEngine.Random.Range(1, 100), false);
			else
				newOrRecycled.childGrid.SetItemsAndUpdate(childAdapterModel.items); // already having the items => update directly
		}

		/// <inheritdoc/>
		protected override void OnBeforeRecycleOrDisableViewsHolder(ChildAdapterViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			inRecycleBinOrVisible.childGrid.SetWaitingForData(false);

			// Cancel any pending download
			if (inRecycleBinOrVisible.modelsDownloadingCoroutine != null)
			{
				StopCoroutine(inRecycleBinOrVisible.modelsDownloadingCoroutine);
				inRecycleBinOrVisible.modelsDownloadingCoroutine = null;
			}

			// Resetting the scroll position to be at start
			// 0f = left for horizontal ScrollViews (using Unity's ScrollRect convention)
			if (inRecycleBinOrVisible.childGrid != null)
				try { inRecycleBinOrVisible.childGrid.SetNormalizedPosition(0f); } catch { }

			inRecycleBinOrVisible.childGrid.ClearItemsAndUpdate();
			inRecycleBinOrVisible.childGrid.StopMovement();

			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
		}
		#endregion

		void TryDownloadingSomeItemsForChildScrollView(ChildAdapterViewsHolder viewsHolder, ChildAdapterModel childScrollViewModel, int count, bool append)
		{
			viewsHolder.childGrid.SetWaitingForData(true);
			//int itemsCountAtRequest = _Params.Data.Count;
			//int vhIndexAtRequest = newOrRecycled.ItemIndex;
			Coroutine coroutine = null;
			int nextFreeId = viewsHolder.childGrid.GetItemsCount(); // the "id" (used only to show it in TitleText) for the first downloaded item is the current items count;
			viewsHolder.modelsDownloadingCoroutine = coroutine = StartCoroutine(SimulateDownloadChildItemsCoroutine(
				count,
				nextFreeId,
				downloadedItems =>
			{
				// Use the results only if the ViewsHolder wasn't recycled meanwhile
				//if (itemsCountAtRequest == _Params.Data.Count
				//	&& newOrRecycled.ItemIndex == vhIndexAtRequest)
				if (viewsHolder.modelsDownloadingCoroutine == null || viewsHolder.modelsDownloadingCoroutine != coroutine)
					return;

				viewsHolder.modelsDownloadingCoroutine = null; // done

				// The childScrollViewModel keeps track of items, because the 
				// child script is not a "persistent storage" - it's being constantly recycled
				if (append) // append is faster
				{
					childScrollViewModel.items.AddRange(downloadedItems);
					viewsHolder.childGrid.AddItemsAndUpdate(downloadedItems);
				}
				else
				{
					childScrollViewModel.items = new List<MyCellModel>(downloadedItems);
					viewsHolder.childGrid.SetItemsAndUpdate(downloadedItems);
				}
			}));
		}

		IEnumerator SimulateDownloadChildItemsCoroutine(int num, int nextFreeId, Action<List<MyCellModel>> onDone)
		{
			yield return null; // at leas 1 frame
			yield return _WaitForSecondsCached;

			var list = new List<MyCellModel>(num);
			for (int i = 0; i < num; i++)
			{
				if (i % 3 == 0)
					yield return null;
				var cellModel = new MyCellModel()
				{
					title = nextFreeId + "",
					image = _Params.availableChildItemsImages[UnityEngine.Random.Range(0, _Params.availableChildItemsImages.Length)]
				};
				++nextFreeId;
				list.Add(cellModel);
			}
			if (onDone != null)
				onDone(list);
		}

		void OnAddRemoveClickedOnChild(ChildAdapterViewsHolder vh)
		{
			if (vh.modelsDownloadingCoroutine != null)
			{
				Debug.Log("Already downloading");
				return;
			}

			var model = Data[vh.ItemIndex];

			if (model.items == null) // waiting for initial init 
				return;

			int addRemoveCount = vh.addRemoveButtonWithInput.InputFieldValueAsInt;
			int currentCount = vh.childGrid.GetItemsCount();
			int newCount = currentCount + addRemoveCount;
			if (newCount < 0 || newCount > MAX_ITEMS_ALLOWED_IN_CHILD)
				return;

			if (addRemoveCount > 0)
			{
				TryDownloadingSomeItemsForChildScrollView(vh, model, addRemoveCount, true);
			}
			else
			{
				int countToRemove = -addRemoveCount;
				// Remove last <countToRemove>
				model.items.RemoveRange(model.items.Count - countToRemove, countToRemove);
				vh.childGrid.RemoveItemsAndUpdate(countToRemove);
			}
		}

		MyCellModel CreateNewChildModelWithIndex(int itemIndex)
		{
			return new MyCellModel()
			{
				title = itemIndex + "",
				image = _Params.availableChildItemsImages[UnityEngine.Random.Range(0, _Params.availableChildItemsImages.Length)]
			};
		}
	}


	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		public Sprite[] availableIntroImages;
		public Sprite[] availableChildItemsImages; // used to randomly generate child models;
	}


	public class ChildAdapterModel
	{
		public string title;
		public Sprite introImage;
		// The models of the child items are stored here, because the whole child view tree is being constantly re-used
		public List<MyCellModel> items = null; // keeping it null so we can know when to download the models for the first time

		//// View state
		//public float normalizedScrollPos = 0f; // 0 = left for horizontal ScrollViews (using Unity's ScrollRect convention)
	}


	public class ChildAdapterViewsHolder : BaseItemViewsHolder
	{
		public Image introImage;
		public Text titleText;
		public ButtonWithInputPanel addRemoveButtonWithInput;
		public ChildGridExample childGrid;

		// Not actually viewstate-related, but it's a convenient place to put it, because it can be stopped when the ViewsHolder is being recycled/disabled
		public Coroutine modelsDownloadingCoroutine;


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
			root.GetComponentAtPath("IntroImagePanel/IntroImage", out introImage);
			root.GetComponentAtPath("ChildGrid", out childGrid);
			root.GetComponentAtPath("ButtonWithInput", out addRemoveButtonWithInput);
		}

		public void UpdateViews(ChildAdapterModel model)
		{
			titleText.text = model.title;
			introImage.sprite = model.introImage;

			//float curPos = childGrid.GetNormalizedPosition();
			//if (curPos != model.normalizedScrollPos)
			//	childGrid.SetNormalizedPosition(model.normalizedScrollPos);
		}
	}
}