using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.Grid
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class GridSceneEntry : DemoSceneEntry<GridExample, GridParams, CellGroupViewsHolder<MyCellViewsHolder>>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false, true, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = .1f;
			_Drawer.freezeContentEndEdgeToggle.onValueChanged.AddListener(OnFreezeContentEndEdgeToggleValueChanged);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			OnFreezeContentEndEdgeToggleValueChanged(_Drawer.freezeContentEndEdgeToggle.isOn);

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(GridExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			adapter.LazyData.ResetItems(count, adapter.freezeContentEndEdgeOnCountChange);
		}
		void OnFreezeContentEndEdgeToggleValueChanged(bool isOn)
		{
			_Adapters[0].freezeContentEndEdgeOnCountChange = isOn;
		}
		#endregion
	}
}
