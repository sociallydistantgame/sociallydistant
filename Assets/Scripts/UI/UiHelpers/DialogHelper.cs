#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using UI.Windowing;
using UnityEngine;
using Utility;

namespace UI.UiHelpers
{
	public class DialogHelper : 
		MonoBehaviour,
		IWindowCloseBlocker
	{
		private readonly List<IMessageDialog> openDialogs = new List<IMessageDialog>();

		[SerializeField]
		private PlayerInstanceHolder player = null!;

		public bool AreAnyDialogsOpen => openDialogs.Any();
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DialogHelper));
		}

		public void AskQuestion(string title, string message, IWindow? parentWindow, Action<bool>? callback)
		{
			IMessageDialog messageDialog = player.Value.UiManager.WindowManager.CreateMessageDialog(title, parentWindow);
			messageDialog.Title = title;
			messageDialog.Message = message;
			messageDialog.Icon = MessageDialogIcon.Question;

			messageDialog.Buttons.Add("Yes");
			messageDialog.Buttons.Add("No");

			void fireCallback(int buttonId)
			{
				messageDialog.ButtonPressed -= fireCallback;
				callback?.Invoke(buttonId==0);
			}

			messageDialog.ButtonPressed += fireCallback;
			
			openDialogs.Add(messageDialog);
		}

		/// <inheritdoc />
		public bool CheckCanClose()
		{
			return !AreAnyDialogsOpen;
		}
	}
}