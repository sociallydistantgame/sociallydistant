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
using Shell.Windowing;
using TMPro;
using UI.Windowing;
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
		private TMP_Dropdown filterSelect = null!;

		[SerializeField]
		private TMP_InputField nameInput = null!;
		
		[SerializeField]
		private Button cancelButton = null!;

		[SerializeField]
		private WidgetList placesList = null!;

		[SerializeField]
		private WidgetList fileList = null!;
		
		public string Filter { get; set; } = string.Empty;
		public string Directory { get; set; } = Environment.CurrentDirectory;
		public ChooserType FileChooserType { get; set; }

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
			nameInput.SetTextWithoutNotify(Path.GetFileName(chosenPath));
			
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
					Selected = dir == chosenPath,
					Callback = Navigate
				});
				
				entryCount ++;
			}
			
			foreach (string dir in files)
			{
				if (!ApplyFilter(Path.GetFileName(dir)))
					continue;
				
				builder.AddWidget(new ListItemWidget<string>
				{
					List = list,
					Data = dir,
					Title = Path.GetFileName(dir),
					Selected = dir == chosenPath,
					Callback = (p) =>
					{
						chosenPath = p;
						nameInput.SetTextWithoutNotify(Path.GetFileName(chosenPath));
					}
				});
				
				entryCount ++;
			}
			
			if (entryCount == 0)
				builder.AddLabel("Directory is empty");
			
			fileList.SetItems(builder.Build());
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
			
			placesList.SetItems(builder.Build());
		}

		private void Navigate(string path)
		{
			chosenPath = path;
			
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

		private async void Select()
		{
			if (!selectButton.enabled)
				return;

			selectButton.enabled = false;

			string path = Path.Combine(Directory, nameInput.text);

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