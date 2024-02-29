#nullable enable
using System;
using GamePlatform;
using Player;
using Shell.Windowing;
using TMPro;
using UI.UiHelpers;
using UnityEngine;
using UnityExtensions;
using UnityEngine.UI;
using System.IO;

namespace UI.Login
{
	public class UserSettingsWindow : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private PlayerInstanceHolder playerHolder = null!;

		[Header("UI")]
		[SerializeField]
		private Button browseButton = null!;

		[SerializeField]
		private Button deleteButton = null!;

		[SerializeField]
		private TextMeshProUGUI fullName = null!;

		[SerializeField]
		private TextMeshProUGUI shellString = null!;

		private DialogHelper dialogHelper = null!;
		private IWindow parentWindow;
		private IGameData? user;
		
		public IGameData? User
		{
			get => user;
			set
			{
				user = value;
				UpdateUI();
			}
		}

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(UserSettingsWindow));
			this.MustGetComponentInParent(out parentWindow);
			this.MustGetComponent(out dialogHelper);
		}

		private void Start()
		{
			deleteButton.onClick.AddListener(Delete);
			browseButton.onClick.AddListener(Browse);
		}

		private void UpdateUI()
		{
			if (user == null)
				return;

			fullName.SetText(user.PlayerInfo.Name);
			shellString.SetText($"{user.PlayerInfo.UserName}@{user.PlayerInfo.HostName}");
		}

		private async void Delete()
		{
			if (user == null)
				return;
			
			if (string.IsNullOrWhiteSpace(user.LocalFilePath))
				return;

			if (playerHolder.Value.UiManager == null)
				return;

			if (!Directory.Exists(user.LocalFilePath))
				return;
			
			var shouldDelete = await dialogHelper.AskQuestionAsync("Delete Account", $"Are you sure you want to delete {user.PlayerInfo.Name}? This action cannot be undone.", null);

			if (!shouldDelete)
				return;

			Directory.Delete(user.LocalFilePath, true);
			parentWindow.ForceClose();
		}

		private void Browse()
		{
			if (user == null)
				return;

			if (string.IsNullOrWhiteSpace(user.LocalFilePath))
				return;

			System.Diagnostics.Process.Start(user.LocalFilePath);
		}
	}
}