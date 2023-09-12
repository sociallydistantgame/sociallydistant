#nullable enable

using System;
using Shell;
using Shell.Common;
using UI.Theming;
using UniRx;
using UnityEngine;

namespace UI.Themes
{
	[CreateAssetMenu(menuName = "Theme Service")]
	public class ThemeService : 
		ScriptableObject,
		IThemeService
	{
		private ReactiveProperty<bool> darkMode = new ReactiveProperty<bool>();

		/// <inheritdoc />
		public bool DarkMode
		{
			get => darkMode.Value;
			set => darkMode.Value = value;
		}

		public OperatingSystemTheme? UserTheme { get; set; }
		
		public IObservable<bool> DarkModeObservable => darkMode;
		
		/// <inheritdoc />
		public SimpleColor GetColor(ShellColorName shellColor)
		{
			throw new System.NotImplementedException();
		}
	}
}