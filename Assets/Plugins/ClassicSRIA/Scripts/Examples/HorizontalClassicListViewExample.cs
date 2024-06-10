using UnityEngine;
using System.Collections.Generic;
using frame8.ScrollRectItemsAdapter.Classic.Examples.Common;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples
{
    /// <summary>Same as <see cref="VerticalClassicListViewExample"/> except it's horizontal, the items are not resize-able</summary>
    public class HorizontalClassicListViewExample : ClassicSRIA<SimpleClientViewsHolder>
	{
		public RectTransform itemPrefab;
		public string[] sampleFirstNames;//, sampleLastNames;
		public string[] sampleLocations;
		public DemoUI demoUI;

		public List<SimpleClientModel> Data { get; private set; }


		#region ClassicSRIA implementation
		protected override void Awake()
		{
			base.Awake();

			Data = new List<SimpleClientModel>();
		}

		protected override void Start()
		{
			base.Start();

			ChangeModelsAndReset(demoUI.SetCountValue);

			demoUI.setCountButton.onClick.AddListener(OnItemCountChangeRequested);
			demoUI.scrollToButton.onClick.AddListener(OnScrollToRequested);
			demoUI.addOneTailButton.onClick.AddListener(() => OnAddItemRequested(true));
			demoUI.addOneHeadButton.onClick.AddListener(() => OnAddItemRequested(false));
			demoUI.removeOneTailButton.onClick.AddListener(() => OnRemoveItemRequested(true));
			demoUI.removeOneHeadButton.onClick.AddListener(() => OnRemoveItemRequested(false));
		}
		
		protected override SimpleClientViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new SimpleClientViewsHolder();
			instance.Init(itemPrefab, itemIndex);

			return instance;
		}

		protected override void UpdateViewsHolder(SimpleClientViewsHolder vh) { vh.UpdateViews(Data[vh.ItemIndex]); }
		#endregion

		#region events from DrawerCommandPanel
		void OnAddItemRequested(bool atEnd)
		{
			int index = atEnd ? Data.Count : 0;
			Data.Insert(index, CreateNewModel(index));
			InsertItems(index, 1, demoUI.freezeContentEndEdge.isOn);
		}
		void OnRemoveItemRequested(bool fromEnd)
		{
			if (Data.Count == 0)
				return;

			int index = fromEnd ? Data.Count - 1 : 0;

			Data.RemoveAt(index);
			RemoveItems(index, 1, demoUI.freezeContentEndEdge.isOn);
		}
		void OnItemCountChangeRequested() { ChangeModelsAndReset(demoUI.SetCountValue); }
		void OnScrollToRequested()
		{
			if (demoUI.ScrollToValue >= Data.Count)
				return;

			demoUI.scrollToButton.interactable = false;
			bool started = SmoothScrollTo(demoUI.ScrollToValue, .75f, .5f, .5f, () => demoUI.scrollToButton.interactable = true);
			if (!started)
				demoUI.scrollToButton.interactable = true;
		}
		#endregion

		void ChangeModelsAndReset(int newCount)
		{
			Data.Clear();
			Data.Capacity = newCount;
			for (int i = 0; i < newCount; i++)
			{
				var model = CreateNewModel(i);
				Data.Add(model);
			}

			ResetItems(Data.Count);
		}

		SimpleClientModel CreateNewModel(int index)
		{
			var model = new SimpleClientModel()
			{
				clientName = sampleFirstNames[CUtil.Rand(sampleFirstNames.Length)],
				location = sampleLocations[CUtil.Rand(sampleLocations.Length)],
			};
			model.SetRandom();

			return model;
		}
	}	
}
