using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using frame8.ScrollRectItemsAdapter.Classic.Examples.Common;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples
{
    public class ClassicGridViewExample : ClassicSRIA<CellViewsHolder>
	{
		public RectTransform itemPrefab;
		public Sprite[] availableImages;
		public DemoUI demoUI;

		public List<CellModel> Data { get; private set; }


		#region ClassicSRIA implementation
		protected override void Awake()
		{
			base.Awake();

			Data = new List<CellModel>();
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
		
		protected override CellViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new CellViewsHolder();
			instance.Init(itemPrefab, itemIndex);

			return instance;
		}

		protected override void UpdateViewsHolder(CellViewsHolder vh)
		{
			var model = Data[vh.ItemIndex];
			vh.titleText.text =  "[#"+vh.ItemIndex+"] " + model.title;
			vh.image.sprite = availableImages[model.imageIndex];
		}
		#endregion

		#region events from DrawerCommandPanel
		void OnAddItemRequested(bool atEnd)
		{
			int index = atEnd ? Data.Count : 0;
			Data.Insert(index, CreateNewModel());
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
				var model = CreateNewModel();
				Data.Add(model);
			}

			ResetItems(Data.Count);
		}

		CellModel CreateNewModel()
		{
			int imgIdx = CUtil.Rand(availableImages.Length);
			var model = new CellModel()
			{
				title = "Image "+ imgIdx,
				imageIndex = imgIdx,
			};

			return model;
		}
	}


	public class CellModel
	{
		public string title;
		public int imageIndex;
	}


	public class CellViewsHolder : CAbstractViewsHolder
	{
		public Text titleText;
		public Image image;


		public override void CollectViews()
		{
			base.CollectViews();

			titleText = root.Find("TitleText").GetComponent<Text>();
			image = root.Find("ImagePanel/Image").GetComponent<Image>();
		}
	}
}
