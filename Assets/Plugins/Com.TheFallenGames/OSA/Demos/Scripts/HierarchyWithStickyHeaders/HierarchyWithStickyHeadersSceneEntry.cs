using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.Demos.Hierarchy;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.Demos.HierarchyWithStickyHeaders
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class HierarchyWithStickyHeadersSceneEntry : DemoSceneEntry<HierarchyExample, MyParams, FileSystemEntryViewsHolder>
	{
		[SerializeField]
		OSAHierarchyStickyHeader _Header = null;

		LabelWithInputPanel _DepthPanel;


		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
			_Drawer.forceLowFPSSetting.gameObject.SetActive(false);
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);
			_DepthPanel = _Drawer.AddLabelWithInputPanel("Max Depth");
			_DepthPanel.inputField.characterLimit = 1;
			_DepthPanel.inputField.onValidateInput = (string text, int charIndex, char addedChar) =>
			{
				if (!char.IsDigit(addedChar) || addedChar == '9' || addedChar == '8')
					return '\0';

				if (addedChar == '0')
					return '1';

				return addedChar;
			};
			_DepthPanel.inputField.text = "3";
			var bpanel = _Drawer.AddButtonsWithOptionalInputPanel("Collapse All", "ExpandAll");
			bpanel.button1.onClick.AddListener(_Adapters[0].CollapseAll);
			bpanel.button2.onClick.AddListener(_Adapters[0].ExpandAll);

		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			var adapter = _Adapters[0];
			_Header.InitWithSameItemViewsHolder(adapter);
			// If the header should have a different model-to-view binding logic, use this instead
			//_Header.InitWithCustomItemViewsHolder<FileSystemEntryViewsHolder>(adapter);

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(HierarchyExample adapter, int _)
		{
			base.OnItemCountChangeRequested(adapter, _);

			adapter.Parameters.maxHierarchyDepth = Mathf.Max(1, _DepthPanel.InputFieldValueAsInt);
			int[] childCounts = new int[adapter.Parameters.maxHierarchyDepth];

			// Child count on first level
			childCounts[0] = UnityEngine.Random.Range(30, 60);

			// Child count on second level
			if (childCounts.Length > 1)
				childCounts[1] = UnityEngine.Random.Range(5, 100);

			// Child count on subsequent levels
			if (childCounts.Length > 2)
			{ 
				for (int i = 2; i < childCounts.Length; i++)
				{
					childCounts[i] = UnityEngine.Random.Range(1, 10);
				}
			}

			adapter.TryGenerateRandomTree(childCounts);
		}
		#endregion
	}
}
