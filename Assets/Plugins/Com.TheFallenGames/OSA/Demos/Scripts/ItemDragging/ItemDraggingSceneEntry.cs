using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;

namespace Com.TheFallenGames.OSA.Demos.ItemDragging
{
	/// <summary>
	/// </summary>
	public class ItemDraggingSceneEntry : DemoSceneEntry<ItemDraggingExample, MyParams, MyViewsHolder>
	{
		[SerializeField]
		ItemDraggingExample _NegativeIDsDeck = null;

		LabelWithToggle worldSpaceToggle;


		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, false, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);
			_Drawer.forceLowFPSSetting.gameObject.SetActive(false);
			worldSpaceToggle = _Drawer.AddLabelWithTogglePanel("World space");
			var canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
			worldSpaceToggle.toggle.onValueChanged.AddListener(isOn =>
			{
				if (isOn)
				{
					if (canvas.renderMode == RenderMode.WorldSpace)
						return;
					canvas.worldCamera = Camera.main;
					canvas.renderMode = RenderMode.WorldSpace;
					canvas.transform.position += (canvas.transform.position - canvas.worldCamera.transform.position).normalized * 10 - canvas.worldCamera.transform.right * 6 + canvas.worldCamera.transform.up * 8;
					var euler = canvas.transform.localEulerAngles;
					euler.x = 15f;
					euler.y = 5f;
					euler.z = 5f;
					canvas.transform.localEulerAngles = euler;
				}
				else
				{
					if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
						return;

					canvas.worldCamera = Camera.main;
					canvas.renderMode = RenderMode.ScreenSpaceCamera;
				}
				Canvas.ForceUpdateCanvases();
			});
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(ItemDraggingExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			// Alternatively, we could construct the list here as a local variable and call adapter.Data.ResetItems directly, passing the new list, 
			// but if we can choose, it's better to reuse the same list that's already there. This is optimal only if using Reset, not partial Inserts/Removes
			adapter.Data.List.Clear();
			int sign = adapter == _NegativeIDsDeck ? -1 : 1;
			for (int i = 0; i < count; ++i)
			{
				var m = new MyModel
				{
					id = (i+1) * sign, // ids starting at 1, -1 respectively, because 0 can't be split
					title = "Date " + DateTime.Now.ToString("hh:mm:ss.fff"),
					color = DemosUtil.GetRandomColor(true)
				};
				adapter.Data.List.Add(m);
			}
			adapter.Data.NotifyListChangedExternally();
		}
		#endregion
	}
}
