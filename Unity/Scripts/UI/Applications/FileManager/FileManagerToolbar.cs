#nullable enable
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

		public UnityEvent backPressed = new UnityEvent();
		public UnityEvent forwardPressed = new UnityEvent();
		public UnityEvent upPressed = new UnityEvent();
		public UnityEvent createDirectoryPressed = new UnityEvent();
		
		public bool CanGoBack
		{
			get => backButton.enabled;
			set => backButton.enabled = value;
		}
		
		public bool CanGoForward
		{
			get =>forwardButton.enabled;
			set => forwardButton.enabled = value;
		}

		public bool CanGoUp
		{
			get => upButton.enabled;
			set => upButton.enabled = value;
		}

		public bool CanMakeDirectories
		{
			get => this.createDirectoryButton.gameObject.activeSelf;
			set => this.createDirectoryButton.gameObject.SetActive(value);
		}
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(FileManagerToolbar));
		}

		private void Start()
		{
			backButton.onClick.AddListener(GoBack);
			forwardButton.onClick.AddListener(GoForward);
			upButton.onClick.AddListener(GoUp);
			createDirectoryButton.onClick.AddListener(CreateDirectory);
		}

		public void UpdateCurrentPath(string path)
		{
			this.currentPathText.SetText(path);
		}

		private void GoBack()
		{
			if (!CanGoBack)
				return;

			backPressed.Invoke();
		}

		private void GoForward()
		{
			if (!CanGoForward)
				return;

			forwardPressed.Invoke();
		}

		private void GoUp()
		{
			if (!CanGoUp)
				return;

			upPressed.Invoke();
		}

		private void CreateDirectory()
		{
			if (!CanMakeDirectories)
				return;
			
			createDirectoryPressed.Invoke();
		}
	}
}