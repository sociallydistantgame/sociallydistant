#nullable enable
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace AcidicGui.Theming
{
	public abstract class ThemeProviderComponent : 
		MonoBehaviour,
		IThemeProvider
	{
		/// <inheritdoc />
		public abstract ITheme GetCurrentTheme();
	}
	
	public abstract class ThemeProviderComponent<TTheme, TThemeEngine> :
		ThemeProviderComponent,
		IObservableThemeProvider<TTheme, TThemeEngine>
		where TTheme : ITheme<TThemeEngine>
		where TThemeEngine : IThemeEngine
	{
		[SerializeField]
		[ReadOnly]
		[HideInInspector]
		private AcidicGUISettings acidicGuiSettings = null!;
		
		/// <inheritdoc />
		public TTheme CurrentTheme => GetCurrentThemeInternal();

		protected abstract TTheme? GetMyTheme(); 
		
		private TTheme GetCurrentThemeInternal()
		{
			TTheme? desiredTheme = GetMyTheme();
			if (desiredTheme != null)
				return desiredTheme;
			
			// Try and get a parent component that we can inherit a theme from
			if (this.transform.parent != null)
			{
				var themeProvider = this.transform.parent.GetComponentInParent<ThemeProviderComponent<TTheme, TThemeEngine>>();

				if (themeProvider != null)
					return themeProvider.CurrentTheme;
			}

			if (acidicGuiSettings == null)
				throw new NotSupportedException("Themes are not supported in this project.");
			
			if (acidicGuiSettings.ThemeSettings == null)
				throw new NotSupportedException("Themes are not supported in this project.");

			if (acidicGuiSettings.ThemeSettings.PreviewTheme is TTheme projectTheme)
				return projectTheme;

			throw new NotSupportedException($"This {this.GetType().Name} doesn't have a theme asset assigned, and is not compatible with the project's default theme!");
		}

		private readonly Callbacks callbacks = new Callbacks();

		protected abstract bool OnChangeTheme(TTheme newTheme);
		
		/// <inheritdoc />
		public IDisposable ObserveCurrentTheme(Action<TTheme> themeChangedCallback)
		{
			// Little defacto Reactive Programming implementation. We don't necessarily have UniRx
			// and System.Reactive doesn't play nice with Unity, so this will at least let users
			// observe theme updates in a reactive way.
			
			IDisposable subscription = this.callbacks.Subscribe(themeChangedCallback);
			themeChangedCallback?.Invoke(this.CurrentTheme);
			return subscription;
		}

		public void ChangeTheme(TTheme newTheme)
		{
			if (OnChangeTheme(newTheme))
				callbacks.Invoke(newTheme);
		}

		/// <inheritdoc />
		public override ITheme GetCurrentTheme()
		{
			return CurrentTheme;
		}


#if UNITY_EDITOR

		private void OnValidate()
		{
			acidicGuiSettings = AcidicGUISettings.GetSettings()!;
		}

#endif

		private class Callbacks
		{
			private readonly List<Action<TTheme>> invocationList = new List<Action<TTheme>>();

			public IDisposable Subscribe(Action<TTheme> callbackDelegate)
			{
				invocationList.Add(callbackDelegate);
				return new Callback(this, callbackDelegate);
			}
			
			private void Remove(Action<TTheme> callbackDelegate)
			{
				invocationList.Remove(callbackDelegate);
			}

			public void Invoke(TTheme theme)
			{
				foreach (Action<TTheme> action in invocationList)
					action.Invoke(theme);
			}
			
			private class Callback : IDisposable
			{
				private readonly Action<TTheme> callbackDelegate;
				private readonly Callbacks callbacks;
				
				public Callback(Callbacks callbacks, Action<TTheme> callbackDelegate)
				{
					this.callbackDelegate = callbackDelegate;
					this.callbacks = callbacks;
				}

				public void Dispose()
				{
					callbacks.Remove(callbackDelegate);
				}
			}
		}
	}
}