#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamePlatform;
using Player;
using TMPro;
using UnityEngine;
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
		private Button startGameButton = null!;

		[SerializeField]
		private Button createUserButton = null!;
		
		[SerializeField]
		private Image avatarImage = null!;

		[SerializeField]
		private TextMeshProUGUI displayName = null!;

		[SerializeField]
		private TextMeshProUGUI usernameText = null!;

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
		
		private void RefreshUserList()
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
			
			this.userListController.SetItems(models);
		}

		private string MakeComment(IGameData data)
		{
			var sb = new StringBuilder();

			sb.Append("<color=#858585FF>Last played:</color> ");
			sb.Append(data.PlayerInfo.LastPlayed.ToLongDateString());
			sb.Append(" @ ");
			sb.Append(data.PlayerInfo.LastPlayed.ToLongTimeString());

			sb.AppendLine();
			sb.Append("<color=#858585FF>Mission:</color> ");

			sb.Append(string.IsNullOrWhiteSpace(data.PlayerInfo.Comment) ? "Prologue" : data.PlayerInfo.Comment);

			return sb.ToString();
		}
		
		private void RefreshMainArea()
		{
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

		private void OnSettings()
		{
			
		}
		
		private void OnQuit()
		{
			
		}

		private void OnGameDataSelected(IGameData? data)
		{
			gameToLoad = data;
			RefreshMainArea();
		}
	}
}