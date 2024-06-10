using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using frame8.ScrollRectItemsAdapter.Classic.Examples.Common;
using frame8.ScrollRectItemsAdapter.Classic.Util;
using System;
using System.Collections;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples
{
    /// <summary>Class (initially) implemented during this YouTube tutorial: https://youtu.be/aoqq_j-aV8I (which is now too old to relate). It demonstrates a simple use case with items that expand/collapse on click</summary>
    public class VerticalClassicListViewExample : ClassicSRIA<SimpleExpandableClientViewsHolder>, CExpandCollapseOnClick.ISizeChangesHandler
	{
		public RectTransform itemPrefab;
		public string[] sampleFirstNames;//, sampleLastNames;
		public string[] sampleLocations;
		public DemoUI demoUI;

		public List<ExpandableSimpleClientModel> Data { get; private set; }

		LayoutElement _PrefabLayoutElement;
		// Used to quickly retrieve the views holder given the gameobject
		Dictionary<RectTransform, SimpleExpandableClientViewsHolder> _MapRootToViewsHolder = new Dictionary<RectTransform, SimpleExpandableClientViewsHolder>();


		#region ClassicSRIA implementation
		protected override void Awake()
		{
			base.Awake();

			Data = new List<ExpandableSimpleClientModel>();
			_PrefabLayoutElement = itemPrefab.GetComponent<LayoutElement>();
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

			StartCoroutine(DelayedClick());
		}

		IEnumerator DelayedClick()
		{
			yield return new WaitForSeconds(.4f);

			if (viewsHolders.Count > 0)
				viewsHolders[Mathf.Min(3, Data.Count - 1)].expandCollapseComponent.OnClicked();
		}

		protected override SimpleExpandableClientViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new SimpleExpandableClientViewsHolder();
			instance.Init(itemPrefab, itemIndex);
			instance.expandCollapseComponent.sizeChangesHandler = this;
			_MapRootToViewsHolder[instance.root] = instance;

			return instance;
		}

		protected override void UpdateViewsHolder(SimpleExpandableClientViewsHolder vh) { vh.UpdateViews(Data[vh.ItemIndex]); }
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

		#region CExpandCollapseOnClick.ISizeChangesHandler implementation
		public bool HandleSizeChangeRequest(RectTransform rt, float newSize)
		{
			_MapRootToViewsHolder[rt].layoutElement.preferredHeight = newSize;
			return true;
		}

		public void OnExpandedStateChanged(RectTransform rt, bool expanded)
		{
			var itemIndex = _MapRootToViewsHolder[rt].ItemIndex;
			Data[itemIndex].expanded = expanded;
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

		ExpandableSimpleClientModel CreateNewModel(int index)
		{
			var model = new ExpandableSimpleClientModel()
			{
				clientName = sampleFirstNames[CUtil.Rand(sampleFirstNames.Length)],
				location = sampleLocations[CUtil.Rand(sampleLocations.Length)],
				nonExpandedSize = _PrefabLayoutElement.preferredHeight
			};
			model.SetRandom();

			return model;
		}
	}	
}
