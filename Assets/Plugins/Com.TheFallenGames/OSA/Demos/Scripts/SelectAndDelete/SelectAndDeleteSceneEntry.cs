using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.SelectAndDelete
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class SelectAndDeleteSceneEntry : DemoSceneEntry<SelectAndDeleteExample, MyGridParams, CellGroupViewsHolder<MyCellViewsHolder>>
	{
		int _CurrentFreeID = 0;


		protected override void InitAdapters()
		{
			base.InitAdapters();

			var adapter = _Adapters[0];
			adapter.LazyData = new LazyDataHelper<BasicModel>(adapter, CreateNewModel);
			adapter.Init();
		}

		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false, true, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;

			_Drawer.freezeContentEndEdgeToggle.onValueChanged.AddListener(OnFreezeContentEndEdgeToggleValueChanged);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			OnFreezeContentEndEdgeToggleValueChanged(_Drawer.freezeContentEndEdgeToggle.isOn);

			_Adapters[0].SelectionModeChanged += OnSelectionModeChanged;

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnAddItemRequested(SelectAndDeleteExample adapter, int index)
		{
			base.OnAddItemRequested(adapter, index);

			// Insert for grids can only be done through a Reset, which NotifyListChangedExternally does
			adapter.LazyData.List.Insert(index, 1);
			adapter.LazyData.NotifyListChangedExternally(adapter.Parameters.freezeContentEndEdgeOnCountChange);
		}
		protected override void OnRemoveItemRequested(SelectAndDeleteExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			if (adapter.CellsCount == 0)
				return;

			// Remove for grids can only be done through a Reset, which NotifyListChangedExternally does
			adapter.LazyData.List.Remove(index, 1);
			adapter.LazyData.NotifyListChangedExternally(adapter.Parameters.freezeContentEndEdgeOnCountChange);
		}
		protected override void OnItemCountChangeRequested(SelectAndDeleteExample adapter, int newCount)
		{
			base.OnItemCountChangeRequested(adapter, newCount);

			_CurrentFreeID = 0;
			adapter.LazyData.ResetItems(newCount, adapter.Parameters.freezeContentEndEdgeOnCountChange);
		}
		void OnFreezeContentEndEdgeToggleValueChanged(bool isOn)
		{
			_Adapters[0].Parameters.freezeContentEndEdgeOnCountChange = isOn;
		}
		#endregion

		void OnSelectionModeChanged(bool selectionModeActive)
		{
			// Don't add/remove items while in selection mode
			_Drawer.addRemoveOnePanel.Interactable = !selectionModeActive;
			_Drawer.addRemoveOneAtIndexPanel.Interactable = !selectionModeActive;
		}

		BasicModel CreateNewModel(int index)
		{
			return new BasicModel()
			{
				id = _CurrentFreeID++,
				//title = "Item ID: " + id,
				color = DemosUtil.GetRandomColor(true)
			};
		}
	}
}
