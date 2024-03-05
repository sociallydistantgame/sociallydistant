#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamePlatform;
using Player;
using Shell.Windowing;
using TMPro;
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
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;

		[SerializeField]
		private PlayerInstanceHolder playerHolder = null!;
		
		[Header("UI")]
		[SerializeField]
		private RectTransform userListAreaTransform = null!;

		[SerializeField]
		private RectTransform currentUserAreaTransform = null!;
		
		[SerializeField]
		private TextMeshProUGUI gameVersion = null!;
		
		[SerializeField]
		private UserListController userListController = null!;

		[SerializeField]
		private Button backButton = null!;
		
		[SerializeField]
		private Button startGameButton = null!;

		[SerializeField]
		private Button manageUserButton = null!;
		
		[SerializeField]
		private Button createUserButton = null!;
		
		[SerializeField]
		private Image avatarImage = null!;

		[SerializeField]
		private TextMeshProUGUI displayName = null!;

		[SerializeField]
		private TextMeshProUGUI usernameText = null!;

		[Header("Prefabs")]
		[SerializeField]
		private UserSettingsWindow userSettingsWindowPrefab = null!;
		
		private IGameData? gameToLoad;
		private TextMeshProUGUI buttonText;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(LoginManager));
			this.startGameButton.MustGetComponentInChildren(out buttonText);

			SetupVersion();
		}

		private IEnumerator Start()
		{
			startGameButton.onClick.AddListener(OnLogin);
			this.createUserButton.onClick.AddListener(OnCreateNewUser);
			this.manageUserButton.onClick.AddListener(ManageUser);
			this.backButton.onClick.AddListener(GoBack);

			this.userListController.GameDataSelected += OnGameDataSelected;
			
			yield return null;
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
			
			if (gameManager.Value != null)
			{
				IOrderedEnumerable<IGameData> gameData = gameManager.Value.ContentManager
					.GetContentOfType<IGameData>()
					.OrderByDescending(x => x.PlayerInfo.LastPlayed);
				
				models.AddRange(gameData.Select(x => new UserListItemModel
				{
					GameData = x,
					Name = x.PlayerInfo.Name,
					Comments = MakeComment(x)
				}));
			}

			if (models.Count == 0)
			{
				if (gameManager.Value!=null)
					await gameManager.Value.StartCharacterCreator();
				return;
			}
			
			this.userListController.SetItems(models);
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
			if (gameManager.Value == null)
				return;
            
			startGameButton.enabled = false;
			
			if (gameToLoad != null)
			{
				this.gameManager.Value.StartGame(gameToLoad);
			}
			else
			{
				this.gameManager.Value.StartCharacterCreator();
			}
		}

		private async void OnCreateNewUser()
		{
			if (gameManager.Value == null)
				return;

			await gameManager.Value.StartCharacterCreator();
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
			if (gameManager.Value != null)
				await gameManager.Value.ContentManager.RefreshContentDatabaseAsync();
			
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