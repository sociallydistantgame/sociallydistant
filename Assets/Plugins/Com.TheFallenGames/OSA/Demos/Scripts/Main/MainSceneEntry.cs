using UnityEngine.UI;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;

namespace Com.TheFallenGames.OSA.Demos.Main
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class MainSceneEntry : DemoSceneEntry<MainExample, MyParams, ClientItemViewsHolder>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(
				_Adapters,
				true, true, true,
				false
			);

			_Drawer.AddLoadCustomSceneButton("Compare to classic ScrollView", "non_optimized");

			var scrollToAndResizeSetting = _Drawer.AddButtonWithInputPanel("ScrollTo & Resize");
			scrollToAndResizeSetting.button.onClick.AddListener(() =>
			{
				int location = scrollToAndResizeSetting.InputFieldValueAsInt;
				if (location < 0)
					return;

				_Drawer.RequestSmoothScrollTo(
					location,
					() =>
					{
						foreach (var a in _Adapters) // using foreach because can't access GetItemViewsHolderIfVisible via IOSA
						{
							var vh = a.GetItemViewsHolderIfVisible(location);
							if (vh != null && vh.expandCollapseButton != null)
								vh.expandCollapseButton.onClick.Invoke();
						}
					}
				);
			});
			scrollToAndResizeSetting.transform.SetSiblingIndex(4);

			_Drawer.freezeItemEndEdgeToggle.onValueChanged.AddListener(OnFreezeItemEndEdgeToggleValueChanged);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			OnFreezeItemEndEdgeToggleValueChanged(_Drawer.freezeItemEndEdgeToggle.isOn);

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		void OnFreezeItemEndEdgeToggleValueChanged(bool isOn)
		{
			foreach (var adapter in _Adapters)
				adapter.Parameters.freezeItemEndEdgeWhenResizing = isOn;
		}

		protected override void OnAddItemRequested(MainExample adapter, int index)
		{
			base.OnAddItemRequested(adapter, index);

			adapter.LazyData.InsertItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnRemoveItemRequested(MainExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			if (index < adapter.LazyData.Count)
				adapter.LazyData.RemoveItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnItemCountChangeRequested(MainExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			adapter.LazyData.ResetItems(count, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		#endregion
	}
}
