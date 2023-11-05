#nullable enable
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Applications.FileManager
{
	public class FileManagerToolbar : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI currentPathText = null!;

		[SerializeField]
		private Button backButton = null!;

		[SerializeField]
		private Button forwardButton = null!;

		[SerializeField]
		private Button upButton = null!;

		[SerializeField]
		private Button createDirectoryButton = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(FileManagerToolbar));
		}

		public void UpdateCurrentPath(string path)
		{
			this.currentPathText.SetText(path);
		}
	}
}