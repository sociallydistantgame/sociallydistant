using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using frame8.Logic.Misc.Other.Extensions;
using UnityEngine.UI;
using System;

namespace Com.TheFallenGames.OSA.Demos.LoopingSpinners
{
	/// <summary>Helper that manages the spinners in this example and delegates commands from the drawer panel to all of them</summary>
	public class LoopingSpinnersSceneEntry : DemoSceneEntry<LoopingSpinnerExample, MyParams, MyItemViewsHolder>
	{
		float DECELERATION_FOR_FIRST = .865f;
		float DECELERATION_FOR_LAST = .430f;


		protected override void InitAdapters()
		{
			base.InitAdapters();

			// Sorting asc by the last character in the name. It's expected to be blabla_1 for the first, blabla_2 for the second etc.
			Array.Sort(_Adapters, (a, b) => a.name[a.name.Length - 1] - b.name[b.name.Length - 1]);
		}

		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, false, false, false, false, true, false);

			_Drawer.galleryEffectSetting.slider.value = .2f;
			// No adding/removing at the head of the list
			_Drawer.addRemoveOnePanel.button2.gameObject.SetActive(false);
			_Drawer.addRemoveOnePanel.button4.gameObject.SetActive(false);

			_Drawer.AddButtonsWithOptionalInputPanel("Simulate slot machine").button1.onClick.AddListener(
				() =>
				{
					int i = 0;
					int adaptersCount = _Drawer.AdaptersCount;
					_Drawer.DoForAllAdapters(
						_ =>
						{
							var adapter = _Adapters[i];

							// The adapters will slow down one after another, not all at once
							float t;
							if (adaptersCount == 1) // avoid div by zero
								t = 0f;
							else
								t = ((float)i / (adaptersCount - 1));

							adapter.BaseParameters.effects.InertiaDecelerationRate = Mathf.Lerp(DECELERATION_FOR_FIRST, DECELERATION_FOR_LAST, t);
							UpdateDecelerationSpinnerValueForAdapter(adapter);

							// Positive velocity meaning it'll scroll towards START (left or top)
							var vel = adapter.Velocity;
							vel[adapter.IsHorizontal ? 0 : 1] = 5000f;
							adapter.Velocity = vel;

							++i;
						}
					);
				}
			);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			foreach (var adapter in _Adapters)
				UpdateDecelerationSpinnerValueForAdapter(adapter);

			_Drawer.RequestChangeItemCountToSpecified();
		}

		protected override void OnItemCountChangeRequested(LoopingSpinnerExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			ChangeItemsCountWithChecksInternal(adapter, count);
		}

		protected override void OnAddItemRequested(LoopingSpinnerExample adapter, int _)
		{
			base.OnAddItemRequested(adapter, _);

			ChangeItemsCountWithChecksInternal(adapter, adapter.GetItemsCount() + 1);
		}

		protected override void OnRemoveItemRequested(LoopingSpinnerExample adapter, int _)
		{
			base.OnRemoveItemRequested(adapter, _);

			ChangeItemsCountWithChecksInternal(adapter, adapter.GetItemsCount() - 1);
		}

		void ChangeItemsCountWithChecksInternal(LoopingSpinnerExample adapter, int count)
		{
			adapter.ChangeItemsCountWithChecks(count);
			int newCount = adapter.GetItemsCount();
			_Drawer.setCountPanel.inputField.text = newCount + "";
		}

		void UpdateDecelerationSpinnerValueForAdapter(LoopingSpinnerExample adapter)
		{
			var slider = adapter.transform.GetComponentAtPath<Slider>("DecelerationRateSlider");
			if (slider)
			{
				slider.value = adapter.Parameters.effects.InertiaDecelerationRate;
				slider.onValueChanged.AddListener(val => adapter.Parameters.effects.InertiaDecelerationRate = val);
				var text = transform.GetComponentAtPath<Text>("DecelerationRateText");
				if (text)
					text.enabled = true;
			}
		}
	}
}