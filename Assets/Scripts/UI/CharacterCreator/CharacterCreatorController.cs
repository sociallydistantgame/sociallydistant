#nullable enable

using System;
using AcidicGui.Mvc;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityExtensions;

namespace UI.CharacterCreator
{
	public class CharacterCreatorController : Controller<CharacterCreatorView>
	{
		[Header("Views")]
		[SerializeField]
		private WelcomeScreen welcomeScreen = null!;
		
		private readonly CharacterCreatorState characterCreatorSTate = new();

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(CharacterCreatorController));
		}

		private async UniTaskVoid Start()
		{
			welcomeScreen.SetData(characterCreatorSTate);	
			await NavigateToAsync(welcomeScreen);
		}
	}
}