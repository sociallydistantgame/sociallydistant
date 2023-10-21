#nullable enable
using System;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace AcidicGui.Theming
{
	public abstract class ThemedElement<TTheme, TThemeEngine, TTHemeProvider> : UIBehaviour
		where TTHemeProvider : ThemeProviderComponent<TTheme, TThemeEngine>
		where TTheme : ITheme<TThemeEngine>
		where TThemeEngine : IThemeEngine
	{
		private TTheme? theme;
		private IDisposable? themeObserver;
		private TTHemeProvider themeProvider;

		protected TTHemeProvider Provider => this.themeProvider;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.MustGetComponentInParent(out themeProvider);
		}

		/// <inheritdoc />
		protected override void Start()
		{
			themeObserver = themeProvider.ObserveCurrentTheme(OnThemeChanged);
			base.Start();
		}

		/// <inheritdoc />
		protected override void OnDestroy()
		{
			themeObserver?.Dispose();
			base.OnDestroy();
		}

		protected abstract void OnUpdateTheme(TTheme theme);
		
		protected void NotifyThemePropertyChanged()
		{
			if (this.theme == null)
				return;

			OnUpdateTheme(theme);
		}
		
		private void OnThemeChanged(TTheme newTheme)
		{
			this.theme = newTheme;
			NotifyThemePropertyChanged();
		}
	}
}