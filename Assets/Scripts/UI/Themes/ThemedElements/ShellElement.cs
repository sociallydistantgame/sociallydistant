#nullable enable
using System;
using AcidicGui.Theming;
using UI.PlayerUI;
using UI.Theming;
using UniRx;
using UnityEditor;

namespace UI.Themes.ThemedElements
{
	public abstract class ShellElement : ThemedElement<OperatingSystemTheme, OperatingSystemThemeEngine, OperatingSystemThemeProvider>
	{
		private IDisposable? darkModeObserver;
		
		/// <inheritdoc />
		protected override void Start()
		{
			darkModeObserver = Provider.ObserveEveryValueChanged(x=>x.UseDarkMode)
				.Subscribe(OnDarkModeChanged);
			base.Start();
		}

		/// <inheritdoc />
		protected override void OnDestroy()
		{
			darkModeObserver?.Dispose();
			base.OnDestroy();
		}

		private void OnDarkModeChanged(bool _)
		{
			NotifyThemePropertyChanged();
		}
	}
}