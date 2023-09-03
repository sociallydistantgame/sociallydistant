using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.GridDifferentItemSizes
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class PackedGridSceneEntry : DemoSceneEntry<PackedGridExample, MyGridParams, CellGroupViewsHolder<MyCellViewsHolder>>
	{
		int _CurrentFreeID;
		LabelWithSliderPanel _CellTypesPanel;
		LabelWithToggle _RandomAspectRatiosTogglePanel;


		protected override void InitAdapters()
		{
			base.InitAdapters();

			var adapter = _Adapters[0];
			adapter.LazyData = new DataHelpers.LazyDataHelper<BasicModel>(adapter, CreateNewModel);
			adapter.Init();
		}

		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, false, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);
			//_Drawer.scrollToPanel.gameObject.SetActive(false); // scrollTo is not accurate for this kind of layout yet, because only scrolling to a group index is possible

			var bigChildrenFirstTogglePanel = _Drawer.AddLabelWithTogglePanel("Big children first");
			bigChildrenFirstTogglePanel.transform.SetAsFirstSibling();
			bigChildrenFirstTogglePanel.toggle.isOn = _Adapters[0].Parameters.biggerChildrenFirst;
			bigChildrenFirstTogglePanel.toggle.onValueChanged.AddListener(isOn =>
			{
				if (_Adapters != null && _Adapters.Length > 0 && _Adapters[0] != null && _Adapters[0].IsInitialized)
				{
					var adapter = _Adapters[0];
					if (adapter.Parameters.biggerChildrenFirst != isOn)
					{
						// Just a hack to force an entire refresh of the adapter, as if it was first initialized.
						// This is needed because the "biggerChildrenFirst" property is read only at initialization, but we want to force 
						// its value to be read & the entire view updated at each press of this toggle
						adapter.Parameters.biggerChildrenFirst = isOn;
						ForceReinitializeKeepingModels(adapter);
					}
				}
			});

			var cellGroupAutoSizePanel = _Drawer.AddLabelWithTogglePanel("Auto cell group size");

			var cellGroupSizePanel = _Drawer.AddLabelWithSliderPanel("");
			cellGroupSizePanel.slider.wholeNumbers = true;

			cellGroupAutoSizePanel.toggle.onValueChanged.AddListener(isOn =>
			{
				if (isOn)
				{
					cellGroupSizePanel.Set(1, OSAConst.MAX_CELLS_PER_GROUP_FACTOR_WHEN_INFERRING, -1);
					cellGroupSizePanel.Init("Group size (multiplier)", "1", OSAConst.MAX_CELLS_PER_GROUP_FACTOR_WHEN_INFERRING + "");
				}
				else
				{
					cellGroupSizePanel.Init("Group size (exact)", "1", "60");
					cellGroupSizePanel.Set(1, 60, 60);
				}

				cellGroupSizePanel.slider.onValueChanged.Invoke(cellGroupSizePanel.slider.value);
			});

			cellGroupSizePanel.slider.onValueChanged.AddListener(value =>
			{
				if (_Adapters != null && _Adapters.Length > 0 && _Adapters[0] != null && _Adapters[0].IsInitialized)
				{
					var adapter = _Adapters[0];
					if (adapter.Parameters.Grid.MaxCellsPerGroup != value)
					{
						if (cellGroupAutoSizePanel.toggle.isOn)
							value = -value;

						adapter.Parameters.Grid.MaxCellsPerGroup = UnityEngine.Mathf.RoundToInt(value);
						ForceReinitializeKeepingModels(adapter);
					}
				}
			});

			cellGroupAutoSizePanel.toggle.isOn = _Adapters[0].Parameters.Grid.MaxCellsPerGroup < 0;
			cellGroupAutoSizePanel.toggle.onValueChanged.Invoke(cellGroupAutoSizePanel.toggle.isOn);

			_CellTypesPanel = _Drawer.AddLabelWithSliderPanel("Cell types", "1", "5");
			_CellTypesPanel.slider.wholeNumbers = true;
			_CellTypesPanel.Set(1, 5, 3);

			_RandomAspectRatiosTogglePanel = _Drawer.AddLabelWithTogglePanel("Random aspect ratios");
			_RandomAspectRatiosTogglePanel.toggle.isOn = false;


			cellGroupAutoSizePanel.transform.SetSiblingIndex(1);
			cellGroupSizePanel.transform.SetSiblingIndex(2);
			_CellTypesPanel.transform.SetSiblingIndex(3);
			_RandomAspectRatiosTogglePanel.transform.SetSiblingIndex(4);
		}

		// Just a hack to force an entire refresh of the adapter, as if it was first initialized.
		// This is needed because some properties are only read at initialization, but we want to force 
		// their value to be read & the entire view updated at each press of this toggle
		void ForceReinitializeKeepingModels(PackedGridExample adapter)
		{
			var originalData = adapter.LazyData;

			// Resetting to 0 count clears everything, including visible items, so nothing will be recycled
			adapter.LazyData = new DataHelpers.LazyDataHelper<BasicModel>(adapter, null);
			adapter.ResetItems(0);

			// Do recalculations since layout values could've beed changed
			adapter.Parameters.PrepareForInit(false);
			adapter.Parameters.InitIfNeeded(adapter);

			// Set back the original data and update the view
			adapter.LazyData = originalData;
			adapter.LazyData.NotifyListChangedExternally();
		}


		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();
			
			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(PackedGridExample adapter, int newCount)
		{
			base.OnItemCountChangeRequested(adapter, newCount);

			_CurrentFreeID = 0;
			adapter.LazyData.ResetItems(newCount);
		}
		#endregion


		BasicModel CreateNewModel(int itemIndex)
		{
			int weight = UnityEngine.Random.Range(1, 1 + UnityEngine.Mathf.Clamp(UnityEngine.Mathf.RoundToInt(_CellTypesPanel.slider.value), 1, 10));
			var m = new BasicModel()
			{
				id = _CurrentFreeID++,
				weight = weight
			};
			m.aspectRatio = _RandomAspectRatiosTogglePanel.toggle.isOn ? UnityEngine.Random.Range(.4f, 1.6f) : 1f;

			return m;
		}
	}
}
