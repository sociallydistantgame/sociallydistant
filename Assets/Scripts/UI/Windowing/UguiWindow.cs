#nullable enable
using System;
using System.Collections.Generic;
using Architecture;
using Shell.Common;
using Shell.Windowing;
using TMPro;
using UI.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Windowing
{
	public class UguiWindow : 
		MonoBehaviour,
		ISelectHandler,
		IDeselectHandler,
		IPointerDownHandler,
		IWindowWithClient<RectTransform>
	{
		[FormerlySerializedAs("dragService")]
		[Header("Dependencies")]
		[SerializeField]
		private WindowFocusService focusService = null!;

		[Header("UI")]
		[SerializeField]
		private CompositeIconWidget iconWidget = null!;
		
		[SerializeField]
		private RectTransform clientArea = null!;

		[SerializeField]
		private TextMeshProUGUI titleText = null!;

		[SerializeField]
		private Button closeButton = null!;

		[SerializeField]
		private Button maximizeButton = null!;

		[SerializeField]
		private Button minimizeButton = null!;

		private static UguiWindow? firstWindow = null!;
		private bool isFirstWindow = false;
		private GameObject? eventSystemFocusedGameObject;
		private WindowState currentWindowState;
		private LayoutElement layoutElement = null!;
		private RectTransform currentClient = null!;
		private RectTransform rectTransform = null!;
		private Vector2 positionBackup;
		private ContentSizeFitter contentSizeFitter = null!;
		private Vector2 anchorMinBackup;
		private Vector2 anchorMaxBackup;
		private Vector2 alignmentBackup;
		private readonly List<IWindowCloseBlocker> closeBlockers = new List<IWindowCloseBlocker>();
		
		public WindowFocusService FocusService => focusService;

		/// <inheritdoc />
		public event Action<IWindow>? WindowClosed;

		/// <inheritdoc />
		public CompositeIcon Icon
		{
			get => iconWidget.Icon;
			set => iconWidget.Icon = value;
		}
		
		/// <inheritdoc />
		public IWorkspaceDefinition Workspace { get; private set; } = null!;
		
		/// <inheritdoc />
		public string Title
		{
			get => titleText.text;
			set => titleText.SetText(value);
		}

		/// <inheritdoc />
		public WindowState WindowState
		{
			get => currentWindowState;
			set
			{
				if (currentWindowState == value)
					return;

				currentWindowState = value;
				UpdateWindowState();
			}
		}

		/// <inheritdoc />
		public bool EnableCloseButton
		{
			get => closeButton.gameObject.activeSelf;
			set => closeButton.gameObject.SetActive(value);
		}

		/// <inheritdoc />
		public bool EnableMaximizeButton
		{
			get => maximizeButton.gameObject.activeSelf;
			set => maximizeButton.gameObject.SetActive(value);
		}

		/// <inheritdoc />
		public bool EnableMinimizeButton
		{
			get => minimizeButton.gameObject.activeSelf;
			set => minimizeButton.gameObject.SetActive(value);
		}

		/// <inheritdoc />
		public bool IsActive
			=> gameObject.activeSelf && transform.IsLastSibling();

		public Vector2 Position
		{
			get => rectTransform.anchoredPosition;
			set => rectTransform.anchoredPosition = value;
		}
		
		/// <inheritdoc />
		public Vector2 MinimumSize
		{
			get => new Vector2(layoutElement.minWidth, layoutElement.minHeight);
			set
			{
				layoutElement.minWidth = (int) value.x;
				layoutElement.minHeight = (int) value.y;
			}
		}

		/// <inheritdoc />
		public RectTransform Client => currentClient;

		public RectTransform RectTransform => rectTransform;

		public RectTransform ClientArea => this.clientArea;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(UguiWindow));
			
			this.MustGetComponent(out rectTransform);
			this.MustGetComponent(out contentSizeFitter);
			this.clientArea.MustGetComponent(out layoutElement);
			
			this.maximizeButton.onClick.AddListener(ToggleMaximize);
			this.minimizeButton.onClick.AddListener(Minimize);
			this.closeButton.onClick.AddListener(Close);

			if (firstWindow == null)
			{
				firstWindow = this;
				isFirstWindow = true;
			}
		}

		private void OnDestroy()
		{
			if (firstWindow == this)
				firstWindow = null;
		}

		private void Update()
		{
			UpdateFocusedWindow();
		}

		private void UpdateFocusedWindow()
		{
			if (!isFirstWindow)
				return;

			if (EventSystem.current == null)
				return;

			if (EventSystem.current.currentSelectedGameObject != this.eventSystemFocusedGameObject)
			{
				this.eventSystemFocusedGameObject = EventSystem.current.currentSelectedGameObject;
				this.CheckNewFocusedWindow();
			}
		}

		private void CheckNewFocusedWindow()
		{
			if (eventSystemFocusedGameObject == null)
			{
				focusService.SetWindow(null);
				return;
			}

			UguiWindow? newWindow = eventSystemFocusedGameObject.GetComponentInParents<UguiWindow>();
			if (newWindow == null)
			{
				focusService.SetWindow(null);
				return;
			}

			newWindow.transform.SetAsLastSibling();
			focusService.SetWindow(newWindow);
		}

		public void Close()
		{
			foreach (IWindowCloseBlocker closeBlocker in closeBlockers)
				if (!closeBlocker.CheckCanClose())
					return;

			ForceClose();
		}

		public void ForceClose()
		{
			WindowClosed?.Invoke(this);
			Destroy(this.gameObject);
		}
		
		public void Minimize()
		{
			WindowState = WindowState.Minimized;
		}

		public void Restore()
		{
			if (this.WindowState == WindowState.Maximized)
				this.ToggleMaximize();
			else if (this.WindowState == WindowState.Normal)
				this.Minimize();
			else
				this.WindowState = WindowState.Normal;
		}
		
		public void ToggleMaximize()
		{
			if (this.WindowState == WindowState.Maximized)
				this.WindowState = WindowState.Normal;
			else
				this.WindowState = WindowState.Maximized;
		}
		
		/// <inheritdoc />
		public void SetClient(RectTransform newClient)
		{
			if (currentClient == newClient)
				return;

			this.closeBlockers.Clear();
			
			if (currentClient != null)
				Destroy(currentClient.gameObject);

			currentClient = newClient;
			currentClient.SetParent(clientArea);

			currentClient.localScale = Vector3.one;
			
			this.closeBlockers.AddRange(this.currentClient.GetComponentsInChildren<IWindowCloseBlocker>(true));
		}

		private void UpdateWindowState()
		{
			switch (this.WindowState)
			{
				case WindowState.Normal:
					this.gameObject.SetActive(true);
					this.contentSizeFitter.enabled = true;

					this.rectTransform.anchorMin = anchorMinBackup;
					this.rectTransform.anchorMax = anchorMaxBackup;
					this.rectTransform.pivot = alignmentBackup;
					this.Position = positionBackup;

					break;
				case WindowState.Minimized:
					anchorMinBackup = rectTransform.anchorMin;
					anchorMaxBackup = rectTransform.anchorMax;
					alignmentBackup = rectTransform.pivot;
					positionBackup = this.Position;
					this.gameObject.SetActive(false);
					
					break;
				case WindowState.Maximized:
					positionBackup = this.Position;
					this.gameObject.SetActive(true);

					this.contentSizeFitter.enabled = false;
					anchorMinBackup = rectTransform.anchorMin;
					anchorMaxBackup = rectTransform.anchorMax;
					alignmentBackup = rectTransform.pivot;

					this.rectTransform.anchorMin = new Vector2(0, 0);
					this.rectTransform.anchorMax = new Vector2(1, 1);
					this.rectTransform.pivot = Vector2.zero;
					Position = Vector2.zero;
					rectTransform.sizeDelta = Vector2.zero;
					
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		/// <inheritdoc />
		public void OnSelect(BaseEventData eventData)
		{
			this.transform.SetAsLastSibling();
		}

		/// <inheritdoc />
		public void OnDeselect(BaseEventData eventData)
		{
			if (EventSystem.current == null)
				return;

			if (EventSystem.current.currentSelectedGameObject == null)
				return;

			UguiWindow? otherWindow = EventSystem.current.currentSelectedGameObject.GetComponentInParent<UguiWindow>();
			if (otherWindow == this)
				this.transform.SetAsLastSibling();
		}

		/// <inheritdoc />
		public void SetWorkspace(IWorkspaceDefinition workspace)
		{
			this.Workspace = workspace;
		}

		/// <inheritdoc />
		public void OnPointerDown(PointerEventData eventData)
		{
			this.transform.SetAsLastSibling();
		}
	}
}