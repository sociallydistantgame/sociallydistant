#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		private UserListController userListController = null!;
		
		[SerializeField]
		private Button quitButton = null!;
		
		[SerializeField]
		private Button manageUsersButton = null!;
		
		[SerializeField]
		private Button settingsButton = null!;

		[SerializeField]
		private Button startGameButton = null!;

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
		}

		private IEnumerator Start()
		{
			quitButton.onClick.AddListener(OnQuit);
			settingsButton.onClick.AddListener(OnSettings);
			manageUsersButton.onClick.AddListener(OnManageUsers);
			startGameButton.onClick.AddListener(OnLogin);
			
			yield return null;
			RefreshUserList();
			RefreshMainArea();
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
					Comments = x.PlayerInfo.Comment
				}));
			}

			if (models.Count > 0)
			{
				models.Add(new UserListItemModel
				{
					Name = "New user"
				});
			}
			
			this.userListController.SetItems(models);
		}

		private void RefreshMainArea()
		{
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

		private void OnManageUsers()
		{
			
		}

		private void OnSettings()
		{
			
		}
		
		private void OnQuit()
		{
			
		}
	}
}