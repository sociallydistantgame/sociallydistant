#nullable enable

using System;
using Core;
using Core.Config;
using GamePlatform;
using Player;
using Shell.Windowing;
using TMPro;
using UI.UiHelpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;
using UnityEngine.UI;
using UniRx;

namespace UI.Controllers
{
	public class StatusBarController : UIBehaviour
	{
		
		private PlayerInstanceHolder player = null!;
		
		[Header("UI")]
		
		private RectTransform userArea = null!;

		
		private TextMeshProUGUI userText = null!;

		
		private Button systemSettingsButton = null!;

		
		private Button logoutButton = null!;

		private GameMode gameMode;
		private IDisposable? gameModeObserver;
		private DialogHelper dialogHelper = null!;
		private GameManager gameManagerHolder = null!;
		
		public string UserInfo
		{
			get => userText.text;
			set => userText.SetText(value);
		}
		
		/// <inheritdoc />
		protected override void Awake()
		{
			gameManagerHolder = GameManager.Instance;
			
			this.AssertAllFieldsAreSerialized(typeof(StatusBarController));
			base.Awake();

			this.MustGetComponent(out dialogHelper);
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();
			
			this.systemSettingsButton.onClick.AddListener(OpenSettings);
			this.logoutButton.onClick.AddListener(OnLogoutClicked);
			
			gameModeObserver = gameManagerHolder.GameModeObservable.Subscribe(OnGameModeChanged);
		}

		/// <inheritdoc />
		protected override void OnDestroy()
		{
			gameModeObserver?.Dispose();
			base.OnDestroy();
		}

		private void OpenSettings()
		{
			if (player.Value.UiManager == null)
				return;

			player.Value.UiManager.OpenSettings();
		}

		private async void OnLogoutClicked()
		{
			bool shouldLogout = await dialogHelper.AskQuestionAsync(
				MessageBoxType.Warning,
				"End session",
				"Are you sure you want to log out and end the current session? All open programs will be closed and any unsaved progress will be lost."
			);

			if (!shouldLogout)
				return;
			
			await gameManagerHolder.EndCurrentGame(true);
			await gameManagerHolder.GoToLoginScreen();
		}

		private void UpdateUI()
		{
			var modSettings = new ModdingSettings(GameManager.Instance.SettingsManager);
			
			this.logoutButton.gameObject.SetActive(gameMode == GameMode.OnDesktop && !modSettings.ModDebugMode);
		}
		
		private void OnGameModeChanged(GameMode newGameMode)
		{
			this.gameMode = newGameMode;
			UpdateUI();
		}
	}
}