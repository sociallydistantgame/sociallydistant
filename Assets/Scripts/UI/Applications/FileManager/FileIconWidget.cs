#nullable enable
using System;
using Architecture;
using TMPro;
using UI.Widgets;
using UnityEngine;
using Utility;
using OS.FileSystems;
using UnityEngine.EventSystems;

namespace UI.Applications.FileManager
{
	public class FileIconWidget : 
		MonoBehaviour,
		IPointerClickHandler
	{
		private string currentPath;
		private VirtualFileSystem vfs;
		
		[SerializeField]
		private CompositeIconWidget icon = null!;
		
		[SerializeField]
		private TextMeshProUGUI text = null!;

		public event Action<string>? OnFileClicked; 

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(FileIconWidget));
		}

		public void SetFilePath(VirtualFileSystem vfs, string path)
		{
			this.vfs = vfs;
			this.currentPath = path;

			this.UpdateUI();
		}

		private void UpdateUI()
		{
			this.text.SetText(PathUtility.GetFileName(currentPath));

			if (vfs.DirectoryExists(currentPath))
			{
				this.icon.Icon = new CompositeIcon
				{
					iconColor = Color.white,
					textIcon = MaterialIcons.Folder
				};
			}
			else
			{
				this.icon.Icon = new CompositeIcon
				{
					iconColor = Color.white,
					textIcon = MaterialIcons.AudioFile
				};
			}
		}

		/// <inheritdoc />
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.clickCount == 2 && eventData.button == PointerEventData.InputButton.Left)
				OnFileClicked?.Invoke(currentPath);
		}
	}
}