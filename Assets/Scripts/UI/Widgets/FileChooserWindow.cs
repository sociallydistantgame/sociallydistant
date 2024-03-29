#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;
using AcidicGui.Widgets;
using Core;
using Shell;
using Shell.Common;
using Shell.Windowing;
using TMPro;
using UI.Applications.FileManager;
using UI.Shell.Common;
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
		private FileManagerToolbar toolbar = null!;
		
		[SerializeField]
		private Button selectButton = null!;

		[SerializeField]
		private TMP_Dropdown filterSelect = null!;

		[SerializeField]
		private TMP_InputField nameInput = null!;
		
		[SerializeField]
		private Button cancelButton = null!;

		[SerializeField]
		private WidgetList placesList = null!;

		[SerializeField]
		private FileGridAdapter fileList = null!;
		
		public string Filter { get; set; } = string.Empty;
		public string Directory { get; set; } = Environment.CurrentDirectory;
		public ChooserType FileChooserType { get; set; }

		private IFileChooserDriver? driver;
		private readonly Stack<string> history = new Stack<string>();
		private readonly Stack<string> future = new Stack<string>();
		private readonly List<ExtensionFilterData> filters = new List<ExtensionFilterData>();
		private Action? cancelCallback;
		private Action<string>? callback;
		private int filterIndex = 0;
		private string chosenPath = string.Empty;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(FileChooserWindow));
		}

		private void Start()
		{
			nameInput.onSubmit.AddListener(OnNameSubmit);
			selectButton.onClick.AddListener(Select);
			cancelButton.onClick.AddListener(Cancel);

			toolbar.backPressed.AddListener(GoBack);
			toolbar.forwardPressed.AddListener(GoForward);
			toolbar.upPressed.AddListener(GoUp);
			toolbar.createDirectoryPressed.AddListener(CreateDirectory);
			fileList.onFileDoubleClicked.AddListener(OnFileDoubleClicked);
			
			BuildFilterList();
			RefreshLists();
		}

		private void BuildFilterList()
		{
			IList<ExtensionFilterData> parsedFilters = ParseFilterList(this.Filter);

			this.filterSelect.onValueChanged.AddListener(OnFilterIndexChanged);
            
			this.filters.Clear();
			this.filters.AddRange(parsedFilters);

			this.filterIndex = 0;
		}

		private void OnFileDoubleClicked(string path)
		{
			if (driver?.DirectoryExists(path) == true)
			{
				Navigate(path);
				return;
			}

			if (driver?.FileExists(path) != true)
				return;

			if (!this.ApplyFilter(Path.GetFileName(path)))
				return;
			
			callback?.Invoke(path);
		}

		private IList<ExtensionFilterData> ParseFilterList(string filter)
		{
			var result = new List<ExtensionFilterData>();

			string[] pipeSeparated = filter.Split('|', StringSplitOptions.RemoveEmptyEntries);

			if (pipeSeparated.Length == 0 || pipeSeparated.Length % 2 != 0)
			{
				result.Add(new ExtensionFilterData
				{
					Name = "All files"
				});
				return result;
			}

			var nameBuilder = new StringBuilder();
			for (int i = 0; i < pipeSeparated.Length; i += 2)
			{
				nameBuilder.Length = 0;
				
				string filterName = pipeSeparated[i];
				string extensionList = pipeSeparated[i + 1];

				string[] extensions = extensionList.Split(',');
                
				nameBuilder.Append(filterName);
				nameBuilder.Append(" (");

				for (var j = 0; j < extensions.Length; j++)
				{
					if (j > 0)
						nameBuilder.Append(", ");
					
					nameBuilder.Append(extensions[j]);
				}
				
				nameBuilder.Append(")");

				result.Add(new ExtensionFilterData()
				{
					Name = nameBuilder.ToString(),
					Extensions = extensions.Select(ConvertToRegexPattern).ToArray()
				});
			}
			
			return result;
		}

		private IEnumerable<string> GetPlaces()
		{
			return driver?.GetCommonPlacePaths() ?? Enumerable.Empty<string>();
		}
		
		private void RefreshLists()
		{
			RefreshFilterDropdown();
			RefreshPlaces();
			RefreshFiles();
		}

		private void RefreshFilterDropdown()
		{
			filterSelect.ClearOptions();
			
			filterSelect.AddOptions(filters.Select(x=>x.Name).ToList());
			filterSelect.SetValueWithoutNotify(filterIndex);
		}
		
		private void RefreshFiles()
		{
			toolbar.UpdateCurrentPath(Directory);
			toolbar.CanGoBack = history.Count > 0;
			toolbar.CanGoForward = future.Count > 0;
			
			nameInput.SetTextWithoutNotify(Path.GetFileName(chosenPath));

			IEnumerable<string> directories = driver?.GetSubDirectories(Directory) ?? Enumerable.Empty<string>();
			IEnumerable<string> files = driver?.GetFiles(Directory) ?? Enumerable.Empty<string>();

			var fileModels = new List<ShellFileModel>();
			
			foreach (string dir in directories)
			{
				fileModels.Add(new ShellFileModel
				{
					Path = dir,
					Name = Path.GetFileName(dir),
					Icon = new CompositeIcon
					{
						textIcon = MaterialIcons.Folder,
						iconColor = Color.white.ToShellColor()
					}
				});
			}
			
			foreach (string dir in files)
			{
				if (!ApplyFilter(Path.GetFileName(dir)))
					continue;
				
				fileModels.Add(new ShellFileModel
				{
					Path = dir,
					Name = Path.GetFileName(dir),
					Icon = new CompositeIcon
					{
						textIcon = MaterialIcons.Description,
						iconColor = Color.white.ToShellColor()
					}
				});
			}
			
			fileList.SetFiles(fileModels);
		}

		private bool ApplyFilter(string filename)
		{
			if (filterIndex < 0 || filterIndex >= filters.Count)
				return true;

			ExtensionFilterData data = filters[filterIndex];

			if (data.Extensions.Length == 0)
				return true;

			return data.Extensions
				.Any(x => Regex.IsMatch(filename, x));
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
					Selected = Directory == place
				}, places);
			}

			IEnumerable<SystemVolume> drives = driver?.GetSystemVolumes()
			                                   ?? Enumerable.Empty<SystemVolume>();

			SectionWidget? devices = null;

			foreach (SystemVolume drive in drives)
			{
				if (devices == null)
					builder.AddSection("Devices", out devices);

				builder.AddWidget(new ListItemWidget<string>()
				{
					Title = drive.VolumeLabel,
					Description = $"{SociallyDistantUtility.GetFriendlyFileSize(drive.FreeSpace)} / {SociallyDistantUtility.GetFriendlyFileSize(drive.TotalSpace)}",
					Callback = Navigate,
					List = list,
					Data = drive.Path,
					Selected = Directory == drive.Path
				}, devices);
			}

			placesList.SetItems(builder.Build());
		}

		private void Navigate(string path)
		{
			Navigate(path, true);
		}
		
		private void Navigate(string path, bool addToHistory)
		{
			chosenPath = path;
			
			if (driver?.DirectoryExists(path) == true)
			{
				if (addToHistory)
				{
					future.Clear();
					history.Push(Directory);
				}

				this.Directory = path;
				RefreshFiles();
			}
		}
		
		public Task<string> GetFilePath(IFileChooserDriver? driver = null)
		{
			this.driver = driver ?? new HostFileChooserDriver();
			
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

		private async void Select()
		{
			if (!selectButton.enabled)
				return;

			selectButton.enabled = false;

			string path = driver.PathCombine(Directory, nameInput.text);

			if (System.IO.Directory.Exists(path))
			{
				if (this.FileChooserType == ChooserType.FolderPicker)
				{
					callback?.Invoke(path);
					selectButton.enabled = true;
					return;
				}
				
				Navigate(path);
				selectButton.enabled = true;
				return;
			}

			if (!File.Exists(path) && this.FileChooserType == ChooserType.Open)
			{
				selectButton.enabled = true;
				return;
			}

			if (this.FileChooserType == ChooserType.Open)
			{
				bool isSupportedType = filters.Any(x => x.Extensions.Length == 0 || x.Extensions.Any(x => Regex.IsMatch(path, x)));
				if (!isSupportedType)
				{
					selectButton.enabled = true;
					return;
				}
			}

			if (File.Exists(path) && this.FileChooserType == ChooserType.Save)
			{
				selectButton.enabled = true;
				return;
			}

			callback?.Invoke(path);
		}

		private void Cancel()
		{
			cancelCallback?.Invoke();
		}

		private string ConvertToRegexPattern(string globPattern)
		{
			var sb = new StringBuilder();
            
			foreach (char character in globPattern)
			{
				if (character == '*')
				{
					sb.Append("(.*)");
					continue;
				}

				if (char.IsLetter(character))
				{
					sb.Append('[');
					sb.Append(char.ToLower(character));
					sb.Append(char.ToUpper(character));
					sb.Append(']');
					continue;
				}

				if (char.IsDigit(character))
				{
					sb.Append(character);
					continue;
				}

				sb.Append('\\');
				sb.Append(character);
			}
            
			return sb.ToString();
		}

		private void OnFilterIndexChanged(int newFilterIndex)
		{
			this.filterIndex = newFilterIndex;
			this.RefreshFiles();
		}

		private void OnNameSubmit(string _)
		{
			Select();
		}

		private void GoBack()
		{
			if (history.Count == 0)
				return;

			string path = history.Pop();
			
			future.Push(Directory);
			Navigate(path, false);
		}

		private void GoForward()
		{
			if (future.Count == 0)
				return;

			string path = future.Pop();
			
			history.Push(Directory);
			Navigate(path, false);
		}

		private void GoUp()
		{
			string? parent = Path.GetDirectoryName(Directory);
			if (string.IsNullOrWhiteSpace(parent))
				return;

			if (!System.IO.Directory.Exists(parent))
				return;
			
			Navigate(parent);
		}

		private async void CreateDirectory()
		{
			
		}
		
		public enum ChooserType
		{
			Open,
			Save,
			FolderPicker
		}

		private class ExtensionFilterData
		{
			public string Name { get; set; } = string.Empty;
			public string[] Extensions { get; set; } = Array.Empty<string>();
		}
	}
}