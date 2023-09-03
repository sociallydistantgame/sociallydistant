using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.Demos.IncrementalItemFetch
{
	/// <summary>
	/// This class demonstrates how items can be appended at the bottom as needed (i.e. when the user acually scrolls there), as opposed to directly downloading all of them.
	/// This is useful if the number of items can't be known beforehand (because of reasons). 
	/// Also, here it's demonstrated how items can be set custom sizes by overriding <see cref="CollectItemsSizes(ItemCountChangeMode, int, int, ItemsDescriptor)"/>
	/// Use this approach if it's impossible to know the total number of items in advance or there's simply too much overhead.
	///	If the number of items IS known, consider using placeholder prefabs with a "Loading..." text on them (which may also make for a nicer UX) while they're being downloaded/processed. 
	///	The placeholder approach is implemented in <see cref="Grid.GridExample"/> - be sure to check it out  if interested.
	/// </summary>
	public class IncrementalItemFetchExample : OSA<MyParams, MyItemViewsHolder>
	{
		public event Action StartedFetching;
		public event Action EndedFetching;

		public SimpleDataHelper<ExampleItemModel> Data { get; private set; }

		bool _Fetching;
		bool _LoadedAll;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<ExampleItemModel>(this);

			base.Start();
		}

		/// <inheritdoc/>
		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			if (_Fetching)
				return;

			if (_LoadedAll)
				return;

			int lastVisibleItemitemIndex = -1;
			if (VisibleItemsCount > 0)
				lastVisibleItemitemIndex = GetItemViewsHolder(VisibleItemsCount - 1).ItemIndex;
			int numberOfItemsBelowLastVisible = Data.Count - (lastVisibleItemitemIndex + 1);

			// If the number of items available below the last visible (i.e. the bottom-most one, in our case) is less than <adapterParams.preFetchedItemsCount>, get more
			if (numberOfItemsBelowLastVisible < _Params.preFetchedItemsCount)
			{
				int newPotentialNumberOfItems = Data.Count + _Params.preFetchedItemsCount;
				if (_Params.totalCapacity > -1) // i.e. the capacity isn't unlimited
					newPotentialNumberOfItems = Mathf.Min(newPotentialNumberOfItems, _Params.totalCapacity);

				if (newPotentialNumberOfItems > Data.Count) // i.e. if we there's enough room for at least 1 more item
					StartPreFetching(newPotentialNumberOfItems - Data.Count);
			}
		}

		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);

			if (changeMode == ItemCountChangeMode.REMOVE || count == 0)
				return;

			if (!_Params.randomSizesForNewItems)
				return;

			int indexOfFirstItemThatWillChangeSize;
			if (changeMode == ItemCountChangeMode.RESET)
				indexOfFirstItemThatWillChangeSize = 0;
			else
				indexOfFirstItemThatWillChangeSize = indexIfInsertingOrRemoving;

			int end = indexOfFirstItemThatWillChangeSize + count;

			itemsDesc.BeginChangingItemsSizes(indexOfFirstItemThatWillChangeSize);
			for (int i = indexOfFirstItemThatWillChangeSize; i < end; ++i)
				// Randomize sizes
				itemsDesc[i] = UnityEngine.Random.Range(_Params.DefaultItemSize / 3, _Params.DefaultItemSize * 3);
			itemsDesc.EndChangingItemsSizes();
		}

		/// <inheritdoc/>
		protected override MyItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyItemViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyItemViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			ExampleItemModel model = Data[newOrRecycled.ItemIndex];

			newOrRecycled.titleText.text = "[" + newOrRecycled.ItemIndex + "] " + model.title;
		}
		#endregion

		public void UpdateCapacity(int newCapacity)
		{
			_Params.totalCapacity = newCapacity;
			// Also reduce the current count, if the new capacity is smaller than it
			if (newCapacity < Data.Count)
			{
				int indexOfFirstRemovedItem = newCapacity;
				int cutInCount = Data.Count - newCapacity;
				//DrawerCommandPanel.Instance.setCountPanel.inputField.text = adapterParams.data.Count + "";
				//ResetItemsCount(newCapacity, DrawerCommandPanel.Instance.freezeContentEndEdgeToggle.isOn);
				RemoveItems(indexOfFirstRemovedItem, cutInCount, _Params.freezeContentEndEdgeOnCountChange);
				_Params.statusText.text = Data.Count + " items";
			}
		}

		// Setting _Fetching to true & starting to fetch 
		void StartPreFetching(int additionalItems)
		{
			_Fetching = true;
			if (StartedFetching != null)
				StartedFetching();
			StartCoroutine(FetchItemModelsFromServer(additionalItems, OnPreFetchingFinished));
		}

		// Updating the models list and notify the adapter that it changed; 
		// it'll call GetItemHeight() for each item and UpdateViewsHolder for the visible ones.
		// Setting _Fetching to false
		void OnPreFetchingFinished(List<ExampleItemModel> models)
		{
			if (models.Count > 0)
				Data.InsertItemsAtEnd(models, _Params.freezeContentEndEdgeOnCountChange);

			_Params.statusText.text = Data.Count + " items";

			if (_LoadedAll)
				_Params.statusText.text += " (server has no more)";

			_Fetching = false;
			if (EndedFetching != null)
				EndedFetching();
		}

		IEnumerator FetchItemModelsFromServer(int maxCount, Action<List<ExampleItemModel>> onDone)
		{
			_Params.statusText.text = "Fetching "+ maxCount + " from server...";

			// Simulating server delay
			yield return new WaitForSeconds(_Params.simulatedServerDelay);

			// Generating some random models

			// Simulates the server either returning the requested number of items or less (meaning it doesn't have more)
			bool serverHasEnough = UnityEngine.Random.Range(0, 20) != 0; // 5% chance to have depleted the data on server

			// If server doesn't have enough, it may return less than maxCount, so we simulate that with a random number between 0 and maxCount+1
			int actualReturnedCount = serverHasEnough ? maxCount : UnityEngine.Random.Range(0, maxCount + 1);

			var results = new List<ExampleItemModel>();
			for (int i = 0; i < actualReturnedCount; ++i)
			{
				var m = new ExampleItemModel();
				m.title = "Item got at " + DateTime.Now.ToString("hh:mm:ss");
				results.Add(m);
			}

			_LoadedAll = results.Count < maxCount;

			onDone(results);
		}
	}


	[Serializable]
	public class ExampleItemModel { public string title; }


	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		public Text statusText;
		public int preFetchedItemsCount;
		[Tooltip("Set to -1 if while fetching <preFetchedItemsCount> items, the adapter shouldn't check for a capacity limit")]
		public int totalCapacity;

		[NonSerialized]
		public bool randomSizesForNewItems;
		[NonSerialized]
		public bool freezeContentEndEdgeOnCountChange;
		[NonSerialized]
		public float simulatedServerDelay;
	}


	public class MyItemViewsHolder : BaseItemViewsHolder
	{
		public Text titleText;


		public override void CollectViews()
		{
			base.CollectViews();

			titleText = root.Find("TitlePanel/TitleText").GetComponent<Text>();
		}
	}
}