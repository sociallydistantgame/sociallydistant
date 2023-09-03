using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.ContentSizeFitter
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class ContentSizeFitterSceneEntry : DemoSceneEntry<ContentSizeFitterExample, MyParams, MyItemViewsHolder>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false, true, false, true);
			_Drawer.galleryEffectSetting.slider.value = 0f;
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
		protected override void OnAddItemRequested(ContentSizeFitterExample adapter, int index)
		{
			base.OnAddItemRequested(adapter, index);

			adapter.LazyData.InsertItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnRemoveItemRequested(ContentSizeFitterExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			if (adapter.LazyData.Count == 0)
				return;

			adapter.LazyData.RemoveItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnItemCountChangeRequested(ContentSizeFitterExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			adapter.LazyData.ResetItems(count, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		void OnFreezeContentEndEdgeToggleValueChanged(bool isOn)
		{
			_Adapters[0].Parameters.freezeContentEndEdgeOnCountChange = isOn;
		}
		#endregion
	}
}
