using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.AnimatedInsertRemove
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class AnimatedInsertRemoveSceneEntry : DemoSceneEntry<AnimatedInsertRemoveExample, MyParams, MyVH>
	{
		protected override void InitAdapters()
		{
			base.InitAdapters();

			var adapter = _Adapters[0];
			adapter.LazyData = new LazyDataHelper<MyModel>(adapter, CreateRandomModel);
			adapter.Init();
		}

		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, false, false, false, false, false, true);
			_Drawer.forceLowFPSSetting.gameObject.SetActive(false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);

			var togglePanel = _Drawer.AddLabelWithTogglePanel("Animation middle pivot");
			togglePanel.toggle.isOn = _Adapters[0].Parameters.itemAnimationPivotMiddle;
			togglePanel.toggle.onValueChanged.AddListener(isOn =>
			{
				if (_Adapters != null && _Adapters.Length > 0)
					_Adapters[0].Parameters.itemAnimationPivotMiddle = isOn;
			});
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnAddItemRequested(AnimatedInsertRemoveExample adapter, int index)
		{
			base.OnAddItemRequested(adapter, index);

			adapter.AnimatedInsert(index, CreateRandomModel(index));
		}
		protected override void OnRemoveItemRequested(AnimatedInsertRemoveExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			if (adapter.LazyData.Count == 0)
				return;

			adapter.AnimatedRemove(index);
		}
		protected override void OnItemCountChangeRequested(AnimatedInsertRemoveExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			adapter.LazyData.ResetItems(count, false);
		}
		#endregion


		MyModel CreateRandomModel(int itemIdex)
		{
			return new MyModel()
			{
				color = DemosUtil.GetRandomColor(true)
			};
		}
	}
}
