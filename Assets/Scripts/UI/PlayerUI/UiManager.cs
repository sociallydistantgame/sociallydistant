#nullable enable

using System;
using System.Linq;
using System.Text;
using Core;
using Core.Config;
using Core.Config.SystemConfigCategories;
using GamePlatform;
using Shell;
using Shell.Windowing;
using TMPro;
using UI.Backdrop;
using UI.CharacterCreator;
using UI.Login;
using UI.Popovers;
using UI.Shell;
using UI.Widgets;
using UI.Windowing;
using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.PlayerUI
{
	public class UiManager : 
		MonoBehaviour,
		IShellContext
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		[Header("Prefabs")]
		[SerializeField]
		private GameObject characterCreatorPrefab = null!;
		
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

		[SerializeField]
		private GameObject systemSettingsPrefab = null!;

		[Header("Dialogs")]
		[SerializeField]
		private FileChooserWindow fileChooser = null!;

		[Header("Fonts")]
		[SerializeField]
		private TMP_FontAsset sansSerifFont = null!;

		[SerializeField]
		private TMP_FontAsset serifFont = null!;

		[SerializeField]
		private TMP_FontAsset monospaceFont = null!;

		private Camera mainCamera = null!;
		private string? lastThemeName;
		private IFloatingGui? settingsWindow;
		private OverlayWorkspace? overlayWorkspace;
		private WindowManager windowManager = null!;
		private PopoverLayer popoverLayer = null!;
		private BackdropController backdrop = null!;
		private GameMode gameMode;
		private IDisposable? gameModeObserver;
		private IDisposable? settingsObserver;

		public BackdropController Backdrop => backdrop;
		public PopoverLayer PopoverLayer => popoverLayer;
		public WindowManager WindowManager => windowManager;
		private CharacterCreatorController? characterCreator;
		
		public Desktop? Desktop { get; private set; }
		public LoginManager? LoginManager { get; private set; }
		public CharacterCreatorController? CharacterCreator => characterCreator;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(UiManager));

			this.mainCamera = Camera.main;
			
			Instantiate(backdropPrefab, this.transform).MustGetComponent(out backdrop);
			Instantiate(this.popoverLayerPrefab, this.transform).MustGetComponent(out popoverLayer);
			Instantiate(windowManagerPrefab, this.transform).MustGetComponent(out windowManager);
		}

		private void OnEnable()
		{
			if (gameManager.Value == null)
				return;
			
			gameModeObserver = gameManager.Value.GameModeObservable.Subscribe(OnGameModeChanged);
			settingsObserver = gameManager.Value.SettingsManager.ObserveChanges(OnSettingsUpdated);
		}

		private void OnDisable()
		{
			settingsObserver?.Dispose();
			gameModeObserver?.Dispose();
			settingsObserver = null;
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

		private void OpenOverlay()
		{
			if (overlayWorkspace != null)
				return;

			overlayWorkspace = this.windowManager.CreateSystemOverlay();
			overlayWorkspace.Closed += OverlayClosed;
		}

		private void OverlayClosed()
		{
			this.overlayWorkspace = null;
		}

		public void OpenSettings()
		{
			if (settingsWindow != null)
				return;

			OpenOverlay();
			settingsWindow = this.overlayWorkspace?.CreateFloatingGui("System Settings");

			if (settingsWindow == null)
				return;

			settingsWindow.WindowClosed += SettingsClosed;

			// Create a RectTransformContent to host settings inside
			var content = new RectTransformContent();
			Instantiate(systemSettingsPrefab, content.RectTransform);

			settingsWindow.ActiveContent.Content = content;
		}

		private void SettingsClosed(IWindow win)
		{
			this.settingsWindow = null;
		}
		
		private void OpenCharacterCreator()
		{
			if (CharacterCreator != null)
				return;
			
			Instantiate(characterCreatorPrefab, this.transform)
				.MustGetComponent(out characterCreator);
		}

		private void CloseCharacterCreator()
		{
			if (characterCreator == null)
				return;
			
			Destroy(characterCreator.gameObject);
			characterCreator = null;
		}
		
		public FileChooserWindow CreateFileChooser(UguiWindow win)
		{
			return Instantiate(this.fileChooser, win.ClientArea);
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

			if (gameMode == GameMode.CharacterCreator)
			{
				OpenCharacterCreator();
			}
			else
			{
				CloseCharacterCreator();
			}
			
			switch (this.gameMode)
			{
				case GameMode.OnDesktop:
                case GameMode.InMission:
                case GameMode.LockScreen: // NYI
					ShowDesktop();
					break;
                default:
	                HideDesktop();
	                break;
			}

			FixCanvasCameras();
		}

		private void FixCanvasScaler(CanvasScaler scaler)
		{
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

			scaler.referenceResolution = new Vector2(1920, 1080);
			scaler.matchWidthOrHeight = 1;
		}
		
		private void FixCanvasCameras()
		{
			Canvas[]? canvases = this.GetComponentsInChildren<Canvas>();

			if (canvases == null)
				return;

			foreach (Canvas canvas in canvases)
			{
				if (canvas.transform.parent != this.transform)
					continue;

				if (!canvas.TryGetComponent(out CanvasScaler scaler))
					scaler = canvas.gameObject.AddComponent<CanvasScaler>();

				FixCanvasScaler(scaler);
				
				canvas.worldCamera = null;
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			}
		}
		
		private void OnSettingsUpdated(ISettingsManager settingsManager)
		{
			// TODO: User interface settings
		}

		public void ShowGameLoadError(Exception ex)
		{
			Debug.LogException(ex);
			
			var sb = new StringBuilder();

			sb.AppendLine("Socially Distant could not load your save file. You have been logged out.");
			sb.AppendLine();
			sb.AppendException(ex);
			
			IMessageDialog dialog = windowManager.CreateMessageDialog("Failed to load game");

			dialog.Title = "System error";
			dialog.Message = sb.ToString();
			
			dialog.Buttons.Add("OK");
		}
	}
}