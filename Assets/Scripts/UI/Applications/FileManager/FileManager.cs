#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Architecture;
using OS.Devices;
using UI.Windowing;
using OS.FileSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Applications.FileManager
{
	public class FileManager :
		MonoBehaviour,
		IProgramOpenHandler

	{
		private readonly List<FileIconWidget> fileIcons = new List<FileIconWidget>();
		private ISystemProcess process = null!;
		private IWindow window = null!;
		private VirtualFileSystem vfs = null!;
		private string currentDirectory = "/";
		private Stack<string> history = new Stack<string>();
		private Stack<string> future = new Stack<string>();
		private ITextConsole console = null!;

		public bool CanGoUp => currentDirectory != "/";
		public bool CanGoBack => history.Any();
		public bool CanGoForward => future.Any();

		[Header("UI")]
		[SerializeField]
		private TextMeshProUGUI currentDirectoryText = null!;

		[SerializeField]
		private Button backButton = null!;
		
		[SerializeField]
		private Button forwardButton = null!;
		
		[SerializeField]
		private Button upButton = null!;

		[SerializeField]
		private RectTransform filesGrid = null!;

		[SerializeField]
		private FileIconWidget fileIconWidgetTemplate = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(FileManager));
			this.fileIconWidgetTemplate.gameObject.SetActive(false);
		}

		private void Start()
		{
			this.backButton.onClick.AddListener(this.GoBack);
			this.forwardButton.onClick.AddListener(this.GoForward);
			this.upButton.onClick.AddListener(GoUp);
			
			this.UpdateUI();
		}

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IWindow window, ITextConsole console)
		{
			this.process = process;
			this.window = window;
			this.vfs = process.User.Computer.GetFileSystem(process.User);
			this.currentDirectory = this.process.WorkingDirectory;
			this.console = console;
		}

		private void UpdateUI()
		{
			this.currentDirectoryText.SetText(this.currentDirectory);
			this.process.WorkingDirectory = currentDirectory;

			this.backButton.enabled = CanGoBack;
			this.forwardButton.enabled = CanGoForward;
			this.upButton.enabled = CanGoUp;
			
			// Get all directories AND files in the current one
			var allEntries = new List<string>();
			allEntries.AddRange(this.vfs.GetDirectories(this.currentDirectory));
			allEntries.AddRange(vfs.GetFiles(this.currentDirectory));
			
			// Delete any icons we don't need.
			for (var i = fileIcons.Count - 1; i >= allEntries.Count; i--)
			{
				FileIconWidget widget = fileIcons[i];
				fileIcons.RemoveAt(i);
				widget.OnFileClicked -= OnFileClicked;

				Destroy(widget.gameObject);
			}
			
			// Create any new ones we DO need.
			for (var i = fileIcons.Count; i < allEntries.Count; i++)
			{
				FileIconWidget widget = Instantiate(this.fileIconWidgetTemplate, this.filesGrid);
				
				widget.OnFileClicked += OnFileClicked;
				widget.gameObject.SetActive(true);

				fileIcons.Add(widget);
			}
			
			// Set file paths for all entries.
			for (var i = 0; i < allEntries.Count; i++)
			{
				fileIcons[i].SetFilePath(vfs, allEntries[i]);
			}
		}

		private void OnFileClicked(string path)
		{
			if (vfs.DirectoryExists(path))
				GoTo(path);
			else if (vfs.IsExecutable(path))
			{
				process.User.Computer.ExecuteProgram(process, console, path, Array.Empty<string>());
			}
		}

		public void GoUp()
		{
			GoTo(PathUtility.GetDirectoryName(currentDirectory));
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