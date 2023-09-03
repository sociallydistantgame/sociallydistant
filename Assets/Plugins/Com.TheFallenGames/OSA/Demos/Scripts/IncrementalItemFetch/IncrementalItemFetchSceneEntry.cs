using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;

namespace Com.TheFallenGames.OSA.Demos.IncrementalItemFetch
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class IncrementalItemFetchSceneEntry : DemoSceneEntry<IncrementalItemFetchExample, MyParams, MyItemViewsHolder>
	{
		LabelWithInputPanel _FetchCountSetting;
		LabelWithToggle _RandomSizesForNewItemsSetting;


		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, false, false, true, true, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;

			_Drawer.freezeContentEndEdgeToggle.onValueChanged.AddListener(OnFreezeContentEndEdgeToggleValueChanged);
			_Drawer.serverDelaySetting.inputField.onEndEdit.AddListener(_ => OnSimulatedServerDelayChanged());

			_FetchCountSetting = _Drawer.AddLabelWithInputPanel("Max items to fetch:");
			_FetchCountSetting.inputField.keyboardType = TouchScreenKeyboardType.NumberPad;
			_FetchCountSetting.inputField.characterLimit = 2;
			_FetchCountSetting.inputField.text = _Adapters[0].Parameters.preFetchedItemsCount + "";
			_FetchCountSetting.inputField.onEndEdit.AddListener(_ => _Adapters[0].Parameters.preFetchedItemsCount = _FetchCountSetting.InputFieldValueAsInt);

			_RandomSizesForNewItemsSetting = _Drawer.AddLabelWithTogglePanel("Random sizes for new items");
			_RandomSizesForNewItemsSetting.toggle.onValueChanged.AddListener(OnRandomSizesForNewItemsToggleValueChanged);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			_Adapters[0].StartedFetching += () => _Drawer.setCountPanel.button.interactable = false;
			_Adapters[0].EndedFetching += () => _Drawer.setCountPanel.button.interactable = true;

			OnFreezeContentEndEdgeToggleValueChanged(_Drawer.freezeContentEndEdgeToggle.isOn);
			OnRandomSizesForNewItemsToggleValueChanged(_RandomSizesForNewItemsSetting.toggle.isOn);
			OnSimulatedServerDelayChanged();

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		protected override void OnItemCountChangeRequested(IncrementalItemFetchExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);
			adapter.UpdateCapacity(count);
		}

		void OnFreezeContentEndEdgeToggleValueChanged(bool isOn)
		{
			_Adapters[0].Parameters.freezeContentEndEdgeOnCountChange = isOn;
		}
		void OnRandomSizesForNewItemsToggleValueChanged(bool isOn)
		{
			_Adapters[0].Parameters.randomSizesForNewItems = isOn;
		}
		void OnSimulatedServerDelayChanged()
		{
			_Adapters[0].Parameters.simulatedServerDelay = _Drawer.serverDelaySetting.InputFieldValueAsInt;
		}
		#endregion

	}
}
