using System.Collections.Generic;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using UnityEngine;

namespace Com.TheFallenGames.OSA.Demos.GridWithCategories
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class GridWithCategoriesSceneEntry : DemoSceneEntry<GridWithCategoriesExample, MyGridParams, CellGroupViewsHolder<MyCellViewsHolder>>
	{
		LabelWithInputPanel _NumCategoriesPanel;
		//LabelWithToggle _RandomAspectRatiosTogglePanel;
		GridWithCategoriesExample _Adapter;


		protected override void InitAdapters()
		{
			base.InitAdapters();

			_Adapter = _Adapters[0];
		}

		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, false, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);
			//_Drawer.scrollToPanel.gameObject.SetActive(false); // scrollTo is not accurate for this kind of layout yet, because only scrolling to a group index is possible

			_NumCategoriesPanel = _Drawer.AddLabelWithInputPanel("Categories");
			_NumCategoriesPanel.transform.SetSiblingIndex(0);
			_NumCategoriesPanel.inputField.text = "99";
			_NumCategoriesPanel.inputField.characterLimit = 2; // 100 catgories max

			// We add a custom SetCount panel, disabling the previous one
			_Drawer.setCountPanel.gameObject.SetActive(false);
			var refreshItemsButtonPanel = _Drawer.AddButtonWithInputPanel("Set max items in category & update");
			refreshItemsButtonPanel.transform.SetSiblingIndex(1);
			refreshItemsButtonPanel.inputField.characterLimit = 5; // max 99999 items in category
			refreshItemsButtonPanel.inputField.text = "13";
			refreshItemsButtonPanel.button.onClick.AddListener(() => OnRegenerateModelsRequested(refreshItemsButtonPanel.InputFieldValueAsInt));

			//_RandomAspectRatiosTogglePanel = _Drawer.AddLabelWithTogglePanel("Random aspect ratios");
			//_RandomAspectRatiosTogglePanel.toggle.isOn = false;
			//_RandomAspectRatiosTogglePanel.transform.SetSiblingIndex(4);

			// Auto click the button refreshing the items
			refreshItemsButtonPanel.button.onClick.Invoke();
		}


		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();
			
			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		void OnRegenerateModelsRequested(int newMaxCountInCategory)
		{
			int numCategories = Mathf.Max(0, _NumCategoriesPanel.InputFieldValueAsInt);
			var categories = GridWithCategoriesDemoDataUtil.CreateRandomCategories(numCategories, newMaxCountInCategory);
			_Adapter.ResetData(categories);
		}
		#endregion
	}
}
