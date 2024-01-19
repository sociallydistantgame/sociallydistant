#nullable enable

using System;
using GamePlatform;
using UI.Popovers;
using UI.UiHelpers;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Shell.TrayActions
{
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(DialogHelper))]
	public class QuitToDesktopAction : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		private Button button = null!;
		private DialogHelper dialogHelper = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(QuitToDesktopAction));
			this.MustGetComponent(out button);
			this.MustGetComponent(out dialogHelper);
		}

		private void Start()
		{
			button.onClick.AddListener(OnButtonClick);
		}

		private void OnButtonClick()
		{
			if (dialogHelper.AreAnyDialogsOpen)
				return;

			dialogHelper.AskQuestion(
				"Quit Socially Distant?",
				"Do you want to quit Socially Distant and return to your host desktop? Your progress will be saved, but any unsaved changes to files will be lost.",
				null,
				OnDialogDismissed
			);
		}

		private void OnDialogDismissed(bool yes)
		{
			if (!yes)
				return;

			if (this.gameManager.Value == null)
			{
				PlatformHelper.QuitToDesktop();
				return;
			}

			gameManager.Value.QuitVM();
		}
	}
}