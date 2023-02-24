#nullable enable
using System;
using System.Collections.Generic;
using Architecture;
using TMPro;
using UI.Widgets;
using UnityEngine;
using Utility;

namespace UI.Windowing
{
	public class UguiMessageDialog :
		MonoBehaviour,
		IMessageDialog
	{
		private IWindow? parentWindow;
		private ObservableList<MessageBoxButtonData> buttonsList = new ObservableList<MessageBoxButtonData>();
		private List<ButtonWidget> widgets = new List<ButtonWidget>();
		private MessageDialogIcon iconType;

		[SerializeField]
		private TextMeshProUGUI textIcon = null!;
		
		[SerializeField]
		private TextMeshProUGUI titleText = null!;

		[SerializeField]
		private TextMeshProUGUI messageText = null!;

		[SerializeField]
		private RectTransform buttonsArea = null!;

		[SerializeField]
		private ButtonWidget buttonWidgetTemplate = null!;
		
		/// <inheritdoc />
		public void Close()
		{
			parentWindow?.Close();
		}

		/// <inheritdoc />
		public string Title
		{
			get => titleText.text;
			set => titleText.text = value;
		}

		/// <inheritdoc />
		public string Message
		{
			get => messageText.text;
			set => messageText.text = value;
		}

		/// <inheritdoc />
		public MessageDialogIcon Icon
		{
			get => iconType;
			set
			{
				if (iconType != value)
				{
					iconType = value;
					UpdateIcon();
				}
			}
		}

		/// <inheritdoc />
		public ObservableList<MessageBoxButtonData> Buttons => buttonsList;

		/// <inheritdoc />
		public event Action<int>? ButtonPressed;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(UguiMessageDialog));
			
			this.buttonsList.ItemAdded += HandleItemAdded;
			this.buttonsList.ItemRemoved += HandleItemRemoved;
		}

		private void HandleItemRemoved(MessageBoxButtonData obj)
		{
			RefreshButtons();
		}

		private void HandleItemAdded(MessageBoxButtonData obj)
		{
			RefreshButtons();
		}

		private void RefreshButtons()
		{
			while (widgets.Count > buttonsList.Count)
			{
				ButtonWidget lastWidget = widgets[^1];
				lastWidget.Clicked -= HandleButtonWidgetClicked;
				Destroy(lastWidget.gameObject);
				widgets.RemoveAt(widgets.Count-1);
			}

			while (widgets.Count < buttonsList.Count)
			{
				ButtonWidget widget = Instantiate(buttonWidgetTemplate, buttonsArea);
				widget.Clicked += HandleButtonWidgetClicked;
				widgets.Add(widget);
				widget.gameObject.SetActive(true);
			}

			for (var i = 0; i < buttonsList.Count; i++)
			{
				MessageBoxButtonData buttonData = buttonsList[i];
				ButtonWidget widget = widgets[i];

				widget.Text = buttonData.Text;
			}
		}

		private void HandleButtonWidgetClicked(ButtonWidget button)
		{
			int index = widgets.IndexOf(button);
			this.ButtonPressed?.Invoke(index);
			Close();
		}

		/// <inheritdoc />
		public void Setup(IWindow window)
		{
			this.parentWindow = window;
			if (window is not IWindowWithClient<RectTransform> rectClientWindow)
			{
				window.Close();
				throw new InvalidOperationException("The parent window of a UGUI-based message dialog must implement IWindowWithClient<RectTransform>!");
			}

			window.EnableMaximizeButton = false;
			window.EnableMaximizeButton = false;
			window.EnableCloseButton = false;
			
			window.MinimumSize = Vector2.zero;
			
			this.MustGetComponent(out RectTransform rect);
			rectClientWindow.SetClient(rect);
		}

		private void UpdateIcon()
		{
			this.textIcon.SetText(iconType switch
			{
				MessageDialogIcon.Information => "\ue1eb",
				MessageDialogIcon.Warning => "\ue8b2",
				MessageDialogIcon.Question => "\ue94c",
				MessageDialogIcon.Error => "\ue000",
				_ => "\ue000"
			});
		}
	}
}