#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AcidicGui.Widgets;
using Shell.Windowing;
using Unity.Profiling.Editor;
using UnityEngine;
using UnityExtensions;
using UnityEngine.UI;

namespace UI.Widgets
{
	public class FileChooserWindow : 
		MonoBehaviour,
		IWindowCloseBlocker
	{
		[SerializeField]
		private Button selectButton = null!;

		[SerializeField]
		private Button cancelButton = null!;

		[SerializeField]
		private WidgetList placesList = null!;

		[SerializeField]
		private WidgetList fileList = null!;
		
		public string Filter { get; set; } = string.Empty;
		public string Directory { get; set; } = Environment.CurrentDirectory;
		public ChooserType FileChooserType { get; set; }

		private Action? cancelCallback;
		private Action<string>? callback;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(FileChooserWindow));
		}

		private void Start()
		{
			selectButton.onClick.AddListener(Select);
			cancelButton.onClick.AddListener(Cancel);

			RefreshLists();
		}

		private IEnumerable<string> GetPlaces()
		{
			if (Application.isEditor)
				yield return Application.dataPath.Replace('/', Path.DirectorySeparatorChar);;
			
			yield return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
			yield return Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar);
		}
		
		private void RefreshLists()
		{
			RefreshPlaces();
			RefreshFiles();
		}

		private void RefreshFiles()
		{
			var builder = new WidgetBuilder();
			builder.Begin();

			var list = new ListWidget
			{
				AllowSelectNone = true
			};

			builder.AddWidget(list);

			string[] directories = System.IO.Directory.GetDirectories(Directory);
			string[] files = System.IO.Directory.GetFiles(Directory);

			var entryCount = 0;
			foreach (string dir in directories)
			{
				builder.AddWidget(new ListItemWidget<string>
				{
					List = list,
					Data = dir,
					Title = Path.GetFileName(dir),
				});
				
				entryCount ++;
			}
			
			foreach (string dir in files)
			{
				builder.AddWidget(new ListItemWidget<string>
				{
					List = list,
					Data = dir,
					Title = Path.GetFileName(dir),
				});

				if (entryCount == 0)
					builder.AddLabel("Directory is empty");
				
				entryCount ++;
			}
			
			fileList.SetItems(builder.Build());
		}
		
		private void RefreshPlaces()
		{
			var builder = new WidgetBuilder();
			builder.Begin();

			var list = new ListWidget
			{
				AllowSelectNone = false
			};

			builder.AddSection("Places", out SectionWidget places)
				.AddWidget(list, places);

			foreach (string place in GetPlaces())
			{
				builder.AddWidget(new ListItemWidget<string>()
				{
					Title = Path.GetFileName(place),
					Callback = Navigate,
					List = list,
					Data = place,
					Selected = Directory.StartsWith(place)
				}, places);
			}
			
			placesList.SetItems(builder.Build());
		}

		private void Navigate(string path)
		{
			if (System.IO.Directory.Exists(path))
			{
				this.Directory = path;
				RefreshLists();
			}
		}
		
		public Task<string> GetFilePath()
		{
			var completionSource = new TaskCompletionSource<string>();

			callback = completionSource.SetResult;
			cancelCallback = () => completionSource.SetResult(string.Empty);
            
			return completionSource.Task;
		}
		
		/// <inheritdoc />
		public bool CheckCanClose()
		{
			cancelCallback?.Invoke();
			return true;
		}

		private void Select()
		{
			
		}

		private void Cancel()
		{
			cancelCallback?.Invoke();
		}
		
		public enum ChooserType
		{
			Open,
			Save,
			FolderPicker
		}
	}
}