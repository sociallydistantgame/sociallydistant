#nullable enable
using System;
using AcidicGui.Widgets;
using GamePlatform;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace UI.Shell
{
	public sealed class ShellLinkHelper : LinkHelper
	{
		[SerializeField]
		private GameManagerHolder gameManager = null!;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ShellLinkHelper));
			base.Awake();
		}

		/// <inheritdoc />
		protected override void OnUriClicked(PointerEventData.InputButton button, Uri uri)
		{
			if (gameManager.Value == null)
				return;

			if (!gameManager.Value.UriManager.IsSchemeRegistered(uri.Scheme))
				return;

			try
			{
				gameManager.Value.UriManager.ExecuteNavigationUri(uri);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);

				gameManager.Value.Shell.ShowInfoDialog($"Shell Error", $"An error occurred during a shell navigation request. This is likely a bug in the game or one of your mods. {{Environment.NewLine}}{{Environment.NewLine}}{ex.Message}{Environment.NewLine}{Environment.NewLine}More info can be found in the game log or by running dmesg in Terminal.");
			}
		}
	}
}