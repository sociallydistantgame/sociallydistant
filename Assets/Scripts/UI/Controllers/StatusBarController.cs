#nullable enable

using System;
using Core;
using GamePlatform;
using Player;
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
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManagerHolder = null!;

		[SerializeField]
		private PlayerInstanceHolder player = null!;
		
		[Header("UI")]
		[SerializeField]
		private RectTransform userArea = null!;

		[SerializeField]
		private TextMeshProUGUI userText = null!;

		[SerializeField]
		private Button systemSettingsButton = null!;

		[SerializeField]
		private Button logoutButton = null!;

		private GameMode gameMode;
		private IDisposable? gameModeObserver;
		private DialogHelper dialogHelper = null!;
		
		public string UserInfo
		{
			get => userText.text;
			set => userText.SetText(value);
		}
		
		/// <inheritdoc />
		protected override void Awake()
		{
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

			if (this.gameManagerHolder.Value == null)
				return;

			gameModeObserver = gameManagerHolder.Value.GameModeObservable.Subscribe(OnGameModeChanged);
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
				"End session",
				"Are you sure you want to log out and end the current session? All open programs will be closed and any unsaved progress will be lost."
			);

			if (!shouldLogout)
				return;

			if (gameManagerHolder.Value == null)
				return;

			await gameManagerHolder.Value.EndCurrentGame();
			await gameManagerHolder.Value.GoToLoginScreen();
		}

		private void UpdateUI()
		{
			this.logoutButton.gameObject.SetActive(gameMode == GameMode.OnDesktop);
		}
		
		private void OnGameModeChanged(GameMode newGameMode)
		{
			this.gameMode = newGameMode;
			UpdateUI();
		}
	}
}