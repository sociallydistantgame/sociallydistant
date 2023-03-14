#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Architecture;
using OS.Devices;
using UI.Windowing;
using OS.FileSystems;
using UnityEngine;
using Utility;

namespace UI.Applications.FileManager
{
	public class FileManager :
		MonoBehaviour,
		IProgramOpenHandler

	{
		private ISystemProcess process = null!;
		private IWindow window = null!;
		private VirtualFileSystem vfs = null!;
		private string currentDirectory = "/";
		private Stack<string> history = new Stack<string>();
		private Stack<string> future = new Stack<string>();

		public bool CanGoBack => history.Any();
		public bool CanGoForward => future.Any();
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(FileManager));
		}

		private void Start()
		{
			this.UpdateUI();
		}

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IWindow window)
		{
			this.process = process;
			this.window = window;
			this.vfs = process.User.Computer.GetFileSystem(process.User);
			this.currentDirectory = this.process.WorkingDirectory;
		}

		private void UpdateUI()
		{
			
		}

		public void GoBack()
		{
			if (!history.Any())
				return;

			string path = history.Pop();
			this.future.Push(currentDirectory);
			this.currentDirectory = path;
			this.UpdateUI();
		}

		public void GoForward()
		{
			if (!future.Any())
				return;

			string path = future.Pop();
			this.history.Push(currentDirectory);
			this.currentDirectory = path;
			this.UpdateUI();
		}

		public void GoTo(string path)
		{
			future.Clear();

			history.Push(currentDirectory);
			this.currentDirectory = path;
			this.UpdateUI();
		}
	}
}