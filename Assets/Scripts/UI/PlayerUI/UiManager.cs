#nullable enable

using System;
using GamePlatform;
using UI.Backdrop;
using UI.Login;
using UI.Popovers;
using UI.Shell;
using UI.Windowing;
using UnityEngine;
using Utility;
using UniRx;

namespace UI.PlayerUI
{
	public class UiManager : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;

		[Header("Prefabs")]
		[SerializeField]
		private GameObject desktopPrefab = null!;

		[SerializeField]
		private GameObject backdropPrefab = null!;

		[SerializeField]
		private GameObject loginScreenPrefab = null!;

		[SerializeField]
		private GameObject windowManagerPrefab = null!;

		[SerializeField]
		private GameObject popoverLayerPrefab = null!;

		private WindowManager windowManager = null!;
		private PopoverLayer popoverLayer = null!;
		private BackdropController backdrop = null!;
		private GameMode gameMode;
		private IDisposable? gameModeObserver;

		public BackdropController Backdrop => backdrop;
		public PopoverLayer PopoverLayer => popoverLayer;
		public WindowManager WindowManager => windowManager;
		
		public Desktop? Desktop { get; private set; }
		public LoginManager? LoginManager { get; private set; }
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(UiManager));

			Instantiate(backdropPrefab, this.transform).MustGetComponent(out backdrop);
			Instantiate(this.popoverLayerPrefab, this.transform).MustGetComponent(out popoverLayer);
			Instantiate(windowManagerPrefab, this.transform).MustGetComponent(out windowManager);
		}

		private void OnEnable()
		{
			if (gameManager.Value == null)
				return;

			gameModeObserver = gameManager.Value.GameModeObservable.Subscribe(OnGameModeChanged);
		}

		private void OnDisable()
		{
			gameModeObserver?.Dispose();
			gameModeObserver = null;
		}

		private void ShowDesktop()
		{
			if (Desktop != null)
				return;

			GameObject desktopObject = Instantiate(desktopPrefab, this.transform);

			this.Desktop = desktopObject.MustGetComponent<Desktop>();
		}

		private void HideDesktop()
		{
			if (this.Desktop == null)
				return;
            
			Destroy(Desktop.gameObject);
			Desktop = null;
		}

		private void ShowLoginScreen()
		{
			if (this.LoginManager != null)
				return;

			GameObject loginManagerObject = Instantiate(loginScreenPrefab, this.transform);
			this.LoginManager = loginManagerObject.MustGetComponent<LoginManager>();
		}

		private void HideLoginScreen()
		{
			if (this.LoginManager == null)
				return;
			
			Destroy(this.LoginManager.gameObject);
			this.LoginManager = null;
		}
		
		
		
		private void OnGameModeChanged(GameMode newGameMode)
		{
			this.gameMode = newGameMode;

			if (this.gameMode == GameMode.AtLoginScreen)
			{
				ShowLoginScreen();
			}
			else
			{
				HideLoginScreen();
			}
			
			switch (this.gameMode)
			{
				case GameMode.OnDesktop:
                case GameMode.InMission:
					ShowDesktop();
					break;
                default:
	                HideDesktop();
	                break;
			}
		}
	}
}