using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.Hierarchy
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class HierarchySceneEntry : DemoSceneEntry<HierarchyExample, MyParams, FileSystemEntryViewsHolder>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
			_Drawer.forceLowFPSSetting.gameObject.SetActive(false);
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);
			var bpanel = _Drawer.AddButtonsWithOptionalInputPanel("Collapse All", "ExpandAll");
			bpanel.button1.onClick.AddListener(_Adapters[0].CollapseAll);
			bpanel.button2.onClick.AddListener(_Adapters[0].ExpandAll);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(HierarchyExample adapter, int _)
		{
			base.OnItemCountChangeRequested(adapter, _);

			adapter.TryGenerateRandomTree();
		}
		#endregion
	}
}
