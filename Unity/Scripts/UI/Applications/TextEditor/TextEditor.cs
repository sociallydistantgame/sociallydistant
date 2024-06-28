#nullable enable

using System;
using System.IO;
using System.Threading.Tasks;
using Architecture;
using Core;
using OS.Devices;
using OS.FileSystems;
using Shell.Windowing;
using TMPro;
using UI.UiHelpers;
using UI.Widgets;
using UnityEngine;
using UnityExtensions;
using UnityEngine.UI;

namespace UI.Applications.TextEditor
{
	public class TextEditor :
		MonoBehaviour,
		IProgramOpenHandler,
		IWindowCloseBlocker
	{
		
		private Button newButton = null!;

		
		private Button openButton = null!;

		
		private Button saveButton = null!;

		
		private Button saveAsButton = null!;

		
		private TextMeshProUGUI currentPathText = null!;
			
		
		private TMP_InputField inputField = null!;

		private DialogHelper dialogHelper = null!;
		private string? fileBeingEdited;
		private ISystemProcess process;
		private IContentPanel win;
		private ITextConsole console;
		private bool hasUnsavedChanges = false;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TextEditor));
			this.MustGetComponent(out dialogHelper);
		}

		private void Start()
		{
			this.dialogHelper.FileChooserDriver = new InGameFileChooserDriver(this.process);
			
			newButton.onClick.AddListener(StartNewFile);
			openButton.onClick.AddListener(Open);
			saveButton.onClick.AddListener(Save);
			saveAsButton.onClick.AddListener(SaveAs);
			
			inputField.onValueChanged.AddListener(OnTextChanged);
		}

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args)
		{
			this.process = process;
			this.win = window;
			this.console = console;
			
			string path = PathUtility.Combine(
				process.WorkingDirectory,
				string.Join(" ", args)
			);
			
			this.OpenFile(path);
		}

		private async void StartNewFile()
		{
			if (hasUnsavedChanges)
			{
				if (hasUnsavedChanges)
				{
					bool shouldContinue = await AskToSave();
					if (!shouldContinue)
						return;
				}
			}

			fileBeingEdited = null;
			hasUnsavedChanges = false;
			inputField.SetTextWithoutNotify(string.Empty);
		}

		private async void Open()
		{
			string openPath = await dialogHelper.OpenFile(
				"Open file",
				process.WorkingDirectory,
				"Text files|*.txt|All files|*.*"
			);

			OpenFile(openPath);
		}

		private async void Save()
		{
			await SaveInternal();
		}

		private async void SaveAs()
		{
			await SaveAsInternal();
		}
		
		private async Task SaveInternal()
		{
			if (string.IsNullOrWhiteSpace(fileBeingEdited))
			{
				await SaveAsInternal();
				return;
			}

			IVirtualFileSystem vfs = process.User.Computer.GetFileSystem(process.User);
			
			vfs.WriteAllText(fileBeingEdited, inputField.text);

			hasUnsavedChanges = false;
		}

		private async Task SaveAsInternal()
		{
			string savePath = await dialogHelper.SaveFile(
				"Save file",
				process.WorkingDirectory,
				"Text files|*.txt|All files|*.*"
			);

			fileBeingEdited = savePath;
			
			IVirtualFileSystem vfs = process.User.Computer.GetFileSystem(process.User);
			vfs.WriteAllText(fileBeingEdited, inputField.text);
			hasUnsavedChanges = false;
		}
		
		private async void OpenFile(string path)
		{
			if (hasUnsavedChanges)
			{
				bool shouldContinue = await AskToSave();
				if (!shouldContinue)
					return;
			}

			IVirtualFileSystem vfs = process.User.Computer.GetFileSystem(process.User);

			if (!vfs.FileExists(path))
				return;

			this.fileBeingEdited = path;

			string text = vfs.ReadAllText(path);

			this.inputField.SetTextWithoutNotify(text);
		}

		private async Task<bool> AskToSave()
		{
			bool? questionResult = await GetDialogResult();

			if (questionResult == null)
				return false;

			if (questionResult == true)
				await SaveInternal();

			return true;
		}

		private Task<bool?> GetDialogResult()
		{
			var source = new TaskCompletionSource<bool?>();

			dialogHelper.AskYesNoCancel(
				MessageBoxType.Question,
				"Unsaved changes",
				"The current document has unsaved changes. Would you like to save before continuing?",
				this.win.Window,
				source.SetResult
			);
			
			return source.Task;
		}

		private void OnTextChanged(string _)
		{
			hasUnsavedChanges = true;
		}

		private async void SaveAndClose()
		{
			if (hasUnsavedChanges)
			{
				bool shouldContinue = await AskToSave();
				if (!shouldContinue)
					return;
			}

			this.win.ForceClose();
		}
		
		/// <inheritdoc />
		public bool CheckCanClose()
		{
			if (!hasUnsavedChanges)
				return true;

			SaveAndClose();
			return false;
		}
	}
}