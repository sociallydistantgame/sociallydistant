#nullable enable
using System;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Config;
using GamePlatform;
using Shell;
using Shell.Windowing;
using TMPro;
using UI.Backdrop;
using UI.Boot;
using UI.CharacterCreator;
using UI.Login;
using UI.Popovers;
using UI.Shell;
using UI.Widgets;
using UI.Windowing;
using UnityEngine;
using UniRx;
using UnityExtensions;
using Utility;

namespace UI.PlayerUI
{
	public class UiManager : 
		MonoBehaviour,
		IShellContext
	{
		[Header("Prefabs")]
		[SerializeField]
		private BootScreen bootScreenPrefab = null!;
		
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

		private static Camera mainCamera = null!;
		private GameManager gameManager = null!;
		private string? lastThemeName;
		private CanvasGroup topGroup;
		private IFloatingGui? settingsWindow;
		private OverlayWorkspace? overlayWorkspace;
		private WindowManager windowManager = null!;
		private PopoverLayer popoverLayer = null!;
		private BackdropController backdrop = null!;
		private GameMode gameMode;
		private IDisposable? gameModeObserver;
		private IDisposable? settingsObserver;
		private GameObject? bootScreen;

		public BackdropController Backdrop => backdrop;
		public PopoverLayer PopoverLayer => popoverLayer;
		public WindowManager WindowManager => windowManager;
		private CharacterCreatorController? characterCreator;
		
		public Desktop? Desktop { get; private set; }
		public LoginManager? LoginManager { get; private set; }
		public CharacterCreatorController? CharacterCreator => characterCreator;

		/// <summary>
		///		Gets or sets whether the UI is in auto-pilot mode. If it is, then all user inputs will be blocked
		///		and the UI can only be interacted with via script.
		/// </summary>
		public bool Autopilot
		{
			get => !topGroup.interactable;
			set
			{
				topGroup.interactable = !value;
				topGroup.blocksRaycasts = !value;
			}
		}
		
		private void Awake()
		{
			gameManager = GameManager.Instance;
			
			this.AssertAllFieldsAreSerialized(typeof(UiManager));
			this.MustGetComponent(out topGroup);

			mainCamera = Camera.main;
			
			Instantiate(backdropPrefab, this.transform).MustGetComponent(out backdrop);
			Instantiate(this.popoverLayerPrefab, this.transform).MustGetComponent(out popoverLayer);
			Instantiate(windowManagerPrefab, this.transform).MustGetComponent(out windowManager);

			topGroup.alpha = 1;
			topGroup.interactable = true;
			topGroup.blocksRaycasts = true;
			topGroup.ignoreParentGroups = true;
		}

		private void OnEnable()
		{
			gameModeObserver = gameManager.GameModeObservable.Subscribe(OnGameModeChanged);
			settingsObserver = gameManager.SettingsManager.ObserveChanges(OnSettingsUpdated);
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

		private void CreateBootScreen()
		{
			if (bootScreen != null)
				return;

			bootScreen = Instantiate(bootScreenPrefab, this.transform).gameObject;
		}

		private void DestroyBootScreen()
		{
			if (bootScreen != null)
				Destroy(bootScreen);

			bootScreen = null;
		}
		
		private void OnGameModeChanged(GameMode newGameMode)
		{
			this.gameMode = newGameMode;

			if (this.gameMode == GameMode.Booting)
			{
				CreateBootScreen();
			}
			else
			{
				DestroyBootScreen();
			}
			
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

			dialog.MessageType = MessageBoxType.Error;
			dialog.Title = "System error";
			dialog.Message = sb.ToString();
			
			dialog.Buttons.Add("OK");
		}

		public static Camera UiCamera => mainCamera;

		/// <inheritdoc />
		public Task ShowInfoDialog(string title, string message)
		{
			var completionSource = new TaskCompletionSource<bool>();
			
			IMessageDialog dialog = windowManager.CreateMessageDialog(title);

			dialog.Title = title;
			dialog.Message = message;
		
			dialog.Buttons.Add("OK");

			dialog.DismissCallback = result =>
			{
				completionSource.SetResult(true);
			};

			return completionSource.Task;
		}
	}
}