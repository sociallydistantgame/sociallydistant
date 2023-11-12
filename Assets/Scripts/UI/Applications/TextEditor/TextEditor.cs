#nullable enable

using System;
using System.IO;
using Architecture;
using Core;
using OS.Devices;
using OS.FileSystems;
using Shell.Windowing;
using TMPro;
using UnityEngine;
using UnityExtensions;

namespace UI.Applications.TextEditor
{
	public class TextEditor :
		MonoBehaviour,
		IProgramOpenHandler
	{
		[SerializeField]
		private TMP_InputField inputField = null!;

		private string? fileBeingEdited;
		private ISystemProcess process;
		private IWindow win;
		private ITextConsole console;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TextEditor));
		}

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IWindow window, ITextConsole console, string[] args)
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

		private void OpenFile(string path)
		{
			// TODO: Handle unsaved changes

			IVirtualFileSystem vfs = process.User.Computer.GetFileSystem(process.User);

			if (!vfs.FileExists(path))
				return;

			this.fileBeingEdited = path;

			string text = vfs.ReadAllText(path);

			this.inputField.SetTextWithoutNotify(text);
		}
	}
}