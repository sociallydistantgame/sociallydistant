#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcidicGui.Widgets;
using GamePlatform;
using Player;
using Shell.Windowing;
using Social;
using TMPro;
using UI.Widgets;
using UI.Widgets.Settings;
using UI.Windowing;
using UnityEngine;
using UnityEngine.Assertions;
using Utility;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Login
{
	public class LoginManager : MonoBehaviour
	{
		
		private PlayerInstanceHolder playerHolder = null!;
		
		[Header("UI")]
		
		private RectTransform userListAreaTransform = null!;

		
		private RectTransform currentUserAreaTransform = null!;
		
		
		private TextMeshProUGUI gameVersion = null!;
		
		
		private WidgetList userListController = null!;

		
		private Button backButton = null!;
		
		
		private Button startGameButton = null!;

		
		private Button manageUserButton = null!;
		
		
		private Button createUserButton = null!;
		
		
		private AvatarWidgetController avatarImage = null!;

		
		private TextMeshProUGUI displayName = null!;

		
		private TextMeshProUGUI usernameText = null!;

		[Header("Prefabs")]
		
		private UserSettingsWindow userSettingsWindowPrefab = null!;
		
		private IGameData? gameToLoad;
		private TextMeshProUGUI buttonText;
		private GameManager gameManager = null!;
		
		private void Awake()
		{
			gameManager = GameManager.Instance;
			
			this.AssertAllFieldsAreSerialized(typeof(LoginManager));
			this.startGameButton.MustGetComponentInChildren(out buttonText);

			SetupVersion();
		}

		private async void Start()
		{
			startGameButton.onClick.AddListener(OnLogin);
			this.createUserButton.onClick.AddListener(OnCreateNewUser);
			this.manageUserButton.onClick.AddListener(ManageUser);
			this.backButton.onClick.AddListener(GoBack);
			
			RefreshUserList();
			RefreshMainArea();
			
			
		}

		private void SetupVersion()
		{
			var sb = new StringBuilder();

			sb.Append("version <b>");
			sb.Append(Application.version);
			sb.Append("</b> (unity ");
			sb.Append(Application.unityVersion);
			sb.Append(")");
			
			gameVersion.SetText(sb);
		}

		private async void RefreshUserList()
		{
			var models = new List<UserListItemModel>();

			IOrderedEnumerable<IGameData> gameData = gameManager.ContentManager
				.GetContentOfType<IGameData>()
				.OrderByDescending(x => x.PlayerInfo.LastPlayed);

			var builder = new WidgetBuilder();
			builder.Begin();

			var useNewGameFlow = true;
			
			foreach (IGameData model in gameData)
			{
				useNewGameFlow = false;
				builder.AddWidget(new ListItemWidget<IGameData>
				{
					Data = model,
					Callback = OnGameDataSelected,
					Title = model.PlayerInfo.Name,
					Description = MakeComment(model),
					Image = new AvatarWidget
					{
						Size = AvatarSize.Small,
						AvatarColor = Color.cyan
					}
				});
			}
			
			if (useNewGameFlow)
			{
				await gameManager.StartCharacterCreator();
				return;
			}

			this.userListController.SetItems(builder.Build());
		}

		private string MakeComment(IGameData data)
		{
			var sb = new StringBuilder();

			sb.Append("<color=#858585FF>Last played:</color> ");
			sb.Append(data.PlayerInfo.LastPlayed.ToShortDateString());
			sb.Append(" @ ");
			sb.Append(data.PlayerInfo.LastPlayed.ToShortTimeString());

			sb.AppendLine();
			sb.Append("<color=#858585FF>Mission:</color> ");

			sb.Append(string.IsNullOrWhiteSpace(data.PlayerInfo.Comment) ? "Prologue" : data.PlayerInfo.Comment);

			return sb.ToString();
		}
		
		private void RefreshMainArea()
		{
			bool isUserSelected = this.gameToLoad != null;
			this.createUserButton.gameObject.SetActive(!isUserSelected);
			this.backButton.gameObject.SetActive(isUserSelected);
			
			this.currentUserAreaTransform.gameObject.SetActive(gameToLoad != null);
			this.userListAreaTransform.gameObject.SetActive(gameToLoad==null);
			
			this.displayName.SetText(gameToLoad?.PlayerInfo.Name ?? "New user");

			if (gameToLoad != null)
			{
				this.usernameText.SetText($"{gameToLoad.PlayerInfo.UserName}@{gameToLoad.PlayerInfo.HostName}");
				buttonText.SetText("Log in");
			}
			else
			{
				this.usernameText.SetText("Create a new user account");
				buttonText.SetText("Start new Career");
			}
		}

		private void OnLogin()
		{
			startGameButton.enabled = false;
			
			if (gameToLoad != null)
			{
				this.gameManager.StartGame(gameToLoad);
			}
			else
			{
				this.gameManager.StartCharacterCreator();
			}
		}

		private async void OnCreateNewUser()
		{
			await gameManager.StartCharacterCreator();
		}

		private void ManageUser()
		{
			if (this.gameToLoad == null)
				return;

			if (playerHolder.Value.UiManager == null)
				return;
			
			if (playerHolder.Value.UiManager.WindowManager == null)
				return;

			// create the window
			WindowManager wm = playerHolder.Value.UiManager.WindowManager;
			OverlayWorkspace overlay = wm.CreateSystemOverlay();
			IFloatingGui window = overlay.CreateFloatingGui($"Manage User: {gameToLoad.PlayerInfo.Name}");
			
			// create the window's content
			var rectContent = new RectTransformContent();
			var userSettingsWindow = Instantiate(userSettingsWindowPrefab, rectContent.RectTransform);
			Assert.IsNotNull(rectContent.RectTransform);

			window.ActiveContent.Content = rectContent;
			userSettingsWindow.User = gameToLoad;
			
			window.WindowClosed += OnUserSettingsClosed;
		}
		
		private async void OnUserSettingsClosed(IWindow obj)
		{
			await gameManager.ContentManager.RefreshContentDatabaseAsync();
			
			this.GoBack();
			this.RefreshUserList();
		}
		
		private void GoBack()
		{
			this.OnGameDataSelected(null);
		}

		private void OnGameDataSelected(IGameData? data)
		{
			gameToLoad = data;
			RefreshMainArea();
		}
	}
}