#nullable enable

using System;
using AcidicGui.Mvc;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityExtensions;
using System.Threading.Tasks;
using Architecture;
using Core;
using Core.DataManagement;
using Core.Systems;
using Core.WorldData;
using GamePlatform;
using TMPro;
using UnityEngine.UI;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace UI.CharacterCreator
{
	public class CharacterCreatorController : Controller<CharacterCreatorView>
	{
		[Header("UI")]
		
		private TextMeshProUGUI titleText = null!;

		
		private TextMeshProUGUI descriptionText = null!;

		
		private Button backButton = null!;

		
		private Button forwardButton = null!;

		
		private TextMeshProUGUI progressText = null!;
		
		[Header("Screens")]
		
		private CharacterCreatorView[] screens = Array.Empty<CharacterCreatorView>();
		
		private readonly CharacterCreatorState state = new();
		private int screenIndex = -1;
		private GameManager gameManager = null!;
		private CanvasGroup canvasGroup;
		private WorldManager worldManager = null!;

		private void Awake()
		{
			gameManager = GameManager.Instance;
			
			// We need to be able to modify protected world state, which cannot be done
			// via IWorldManager.
			worldManager = WorldManager.Instance;
			
			this.AssertAllFieldsAreSerialized(typeof(CharacterCreatorController));
			this.MustGetComponent(out canvasGroup);
		}

		private async UniTaskVoid Start()
		{
			if (this.screens.Length == 0)
				return;

			await Task.Yield();
			
			this.backButton.onClick.AddListener(OnBackButtonClicked);
			this.forwardButton.onClick.AddListener(OnForwardButtonClicked);
			
			UpdateUI();
			await ShowScreenAtIndex(0);
		}

		private void Update()
		{
			if (CurrentView == null)
				return;

			this.forwardButton.enabled = CurrentView.CanGoForward;
		}

		private async Task ShowScreenAtIndex(int index)
		{
			if (index < 0 || index >= this.screens.Length)
				throw new ArgumentOutOfRangeException(nameof(index));
            
			this.screenIndex = index;
			
			this.screens[screenIndex].SetData(this.state);

			canvasGroup.interactable = false;
			await this.ReplaceViewsAsync(this.screens[this.screenIndex]);
			canvasGroup.interactable = true;

			UpdateUI();
		}

		private void UpdateUI()
		{
			if (this.CurrentView == null)
			{
				this.titleText.SetText(string.Empty);
				this.descriptionText.SetText(string.Empty);
				this.progressText.SetText(string.Empty);

				this.backButton.gameObject.SetActive(false);
				this.forwardButton.gameObject.SetActive(false);
			}
			else
			{
				this.titleText.SetText(CurrentView.Title);
				this.descriptionText.SetText(this.CurrentView.Description);
				this.progressText.SetText($"{screenIndex + 1} of {screens.Length}");
				
				this.backButton.gameObject.SetActive(this.screenIndex > 0);
				this.forwardButton.gameObject.SetActive(true);
			}
		}

		private void OnBackButtonClicked()
		{
			this.ShowScreenAtIndex(this.screenIndex - 1);
		}

		private async void OnForwardButtonClicked()
		{
			if (CurrentView != null)
			{
				bool shouldContinue = await CurrentView.ConfirmNextPage();

				if (!shouldContinue)
					return;
			}

			if (screenIndex + 1 < screens.Length)
				await ShowScreenAtIndex(screenIndex + 1);
			else
				await StartGame();
		}

		private async Task StartGame()
		{
			// Hide the screen
			await GoBackAsync();
			
			// Make us non-interactible
			canvasGroup.interactable = false;
			
			// Create PlayerInfo structure needed by LocalGameData
			var playerInfo = new PlayerInfo
			{
				UserName = state.UserName!,
				PlayerGender = state.ChosenGender,
				HostName = state.HostName!,
				Name = state.PlayerName!
			};
			
			// Other info is stored in the world. Create an empty WorldManager to save the data to.
			var world = new World(new UniqueIntGenerator(), new DataEventDispatcher(GameManager.Instance));
		
			// Set the lifepath
			world.ChangePlayerLifepath(state.Lifepath!);
			
			// Save the world and player data.
			var gameData = await LocalGameData.CreateNewGame(playerInfo, world);
			
			// Start the game!
			await gameManager.ContentManager.RefreshContentDatabaseAsync();
			await gameManager.StartGame(gameData);
		}
	}
}