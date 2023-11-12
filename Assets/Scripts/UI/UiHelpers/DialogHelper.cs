#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Player;
using Shell.Windowing;
using UI.Widgets;
using UI.Windowing;
using UnityEngine;
using UnityExtensions;
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
		
		public IFileChooserDriver? FileChooserDriver { get; set; }
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DialogHelper));
		}

		public async Task<string> OpenFile(string title, string directory, string extensionFilter)
		{
			OverlayWorkspace overlay = this.player.Value.UiManager.WindowManager.CreateSystemOverlay();
			IWindow win = overlay.CreateWindow(title);
			if (win is not UguiWindow guiWin)
				return string.Empty;

			FileChooserWindow chooser = player.Value.UiManager.CreateFileChooser(guiWin);
			chooser.FileChooserType = FileChooserWindow.ChooserType.Open;
			chooser.Directory = directory;
			chooser.Filter = extensionFilter;

			string result = await chooser.GetFilePath(FileChooserDriver);

			win.ForceClose();
			
			return result;
		}
		
		public void AskYesNoCancel(string title, string message, IWindow? parentWindow, Action<bool?> callback)
		{
			IMessageDialog messageDialog = player.Value.UiManager.WindowManager.CreateMessageDialog(title, parentWindow);
			messageDialog.Title = title;
			messageDialog.Message = message;
			messageDialog.Icon = MessageDialogIcon.Question;

			messageDialog.Buttons.Add("Yes");
			messageDialog.Buttons.Add("No");
			messageDialog.Buttons.Add("Cancel");


			messageDialog.ButtonPressed += FireCallback;
			
			openDialogs.Add(messageDialog);
			return;

			void FireCallback(int buttonId)
			{
				messageDialog.ButtonPressed -= FireCallback;

				switch (buttonId)
				{
					case 0:
						callback?.Invoke(true);
						break;
					case 1:
						callback?.Invoke(false);
						return;
					default:
						callback?.Invoke(null);
						return;
				}
			}
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

		public void ShowMessage(string title, string message, IWindow? parentWindow, Action callback)
		{
			IMessageDialog messageDialog = player.Value.UiManager.WindowManager.CreateMessageDialog(title, parentWindow);
			messageDialog.Title = title;
			messageDialog.Message = message;
			messageDialog.Icon = MessageDialogIcon.Information;

			messageDialog.Buttons.Add("OK");
            
			messageDialog.ButtonPressed += _ =>
			{
				callback?.Invoke();
			};
			
			openDialogs.Add(messageDialog);
		}
		
		/// <inheritdoc />
		public bool CheckCanClose()
		{
			return !AreAnyDialogsOpen;
		}
	}
}