using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Windowing
{
	public class UguiWindow : MonoBehaviour, IWindow<RectTransform>
	{
		private WindowState currentWindowState;
		private LayoutElement layoutElement;
		private RectTransform currentClient;
		
		[Header("UI")]
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
			get => closeButton.enabled;
			set => closeButton.enabled = value;
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

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(UguiWindow));
			this.clientArea.MustGetComponent(out layoutElement);
		}

		/// <inheritdoc />
		public void SetClient(RectTransform newClient)
		{
			if (currentClient != null)
				Destroy(currentClient.gameObject);

			currentClient = newClient;
			currentClient.SetParent(clientArea);
		}

		private void UpdateWindowState()
		{
			
		}
	}
}