#nullable enable
using System;
using System.Collections.Generic;
using Architecture;
using Shell;
using Shell.Common;
using Shell.Windowing;
using TMPro;
using UI.Controllers;
using UI.Widgets;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace UI.Windowing
{
	public class UguiMessageDialog :
		MonoBehaviour,
		IMessageDialog
	{
		[SerializeField]
		private InfoBoxController infoBoxController = null!;
		
		private IWindow? parentWindow;
		private ObservableList<MessageBoxButtonData> buttonsList = new ObservableList<MessageBoxButtonData>();
		
		/// <inheritdoc />
		public void Close()
		{
			parentWindow?.Close();
		}

		/// <inheritdoc />
		public event Action<IWindow>? WindowClosed
		{
			add
			{
				parentWindow!.WindowClosed += value;
			}
			remove
			{
				parentWindow!.WindowClosed -= value;
			}
		}

		/// <inheritdoc />
		public IWorkspaceDefinition? Workspace
		{
			get => parentWindow?.Workspace;
			set
			{
				if (parentWindow == null)
					return;
				
				parentWindow.Workspace = value;
			}
		}

		/// <inheritdoc />
		public string Title
		{
			get => infoBoxController.Title;
			set => infoBoxController.Title = value;
		}

		/// <inheritdoc />
		public CompositeIcon Icon
		{
			get => parentWindow?.Icon ?? default;
			set
			{
				if (parentWindow == null)
					return;

				parentWindow.Icon = value;
			}
		}
		
		/// <inheritdoc />
		public bool IsActive => parentWindow?.IsActive == true;
		
		/// <inheritdoc />
		public void ForceClose()
		{
			this.parentWindow?.ForceClose();
		}
		
		/// <inheritdoc />
		public string Message
		{
			get => infoBoxController.Text;
			set => infoBoxController.Text = value;
		}

		/// <inheritdoc />
		public CommonColor Color
		{
			get => infoBoxController.Color;
			set => infoBoxController.Color = value;
		}

		/// <inheritdoc />
		public IList<MessageBoxButtonData> Buttons => buttonsList;

		/// <inheritdoc />
		public Action<MessageDialogResult>? DismissCallback { get; set; }
		
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
			for (var i = 0; i < buttonsList.Count; i++)
			{
				MessageBoxButtonData definition = buttonsList[i];

				var underlyingButtonDefinition = new InfoBoxController.ButtonDefinition()
				{
					label = definition.Text,
					clickHandler = () => HandleButtonClick(definition.Result)
				};

				if (infoBoxController.Count <= i)
					infoBoxController.Add(underlyingButtonDefinition);
				else
					infoBoxController[i] = underlyingButtonDefinition;
			}

			while (infoBoxController.Count > buttonsList.Count)
				infoBoxController.RemoveAt(infoBoxController.Count - 1);
		}

		private void HandleButtonClick(MessageDialogResult result)
		{
			DismissCallback?.Invoke(result);
			Close();
		}
		
		/// <inheritdoc />
		public void Setup(IWindow window)
		{
			if (window is not IFloatingGuiWithClient<RectTransform> rectWindow)
				throw new InvalidOperationException("fuck");
			
			this.parentWindow = window;
			
			rectWindow.EnableMaximizeButton = false;
			rectWindow.EnableMinimizeButton = false;
			rectWindow.EnableCloseButton = false;
			
			rectWindow.MinimumSize = Vector2.zero;
			
			this.MustGetComponent(out RectTransform rect);
			rectWindow.SetClient(rect);
		}
	}
}