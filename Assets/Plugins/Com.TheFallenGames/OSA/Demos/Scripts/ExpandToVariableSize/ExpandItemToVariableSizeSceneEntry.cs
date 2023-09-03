using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.ExpandToVariableSize
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class ExpandItemToVariableSizeSceneEntry : DemoSceneEntry<ExpandItemToVariableSizeExample, MyParams, MyVH>
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
			_Drawer.Init(_Adapters, false, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(ExpandItemToVariableSizeExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			adapter.LazyData.ResetItems(count, false);
		}
		#endregion


		MyModel CreateRandomModel(int itemIdex)
		{
			return new MyModel()
			{
				//Title = LOREM_IPSUM.Substring(400),
				Title = DemosUtil.GetRandomTextBody((DemosUtil.LOREM_IPSUM.Length) / 15 + 1, (int)(DemosUtil.LOREM_IPSUM.Length * 1.7f))
			};
		}
	}
}
