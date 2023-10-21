#nullable enable
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	[RequireComponent(typeof(ISelectableUpdater), typeof(Toggle))]
	public class ListItemHighlightHelper : MonoBehaviour
	{
		private Toggle toggle;
		private ISelectableUpdater updater;
		private IDisposable? toggleObserver;

		private void Awake()
		{
			this.MustGetComponent(out toggle);
			this.MustGetComponent(out updater);
		}

		private void Start()
		{
			toggleObserver = toggle.ObserveEveryValueChanged(x => x.isOn)
				.Subscribe(OnToggleValueChanged);
		}

		private void OnDestroy()
		{
			toggleObserver?.Dispose();
		}

		private void OnToggleValueChanged(bool value)
		{
			updater.UseActiveAsIdle = value;
		}
	}
}