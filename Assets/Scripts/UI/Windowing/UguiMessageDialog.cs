#nullable enable
using System;
using System.Collections.Generic;
using Architecture;
using Shell;
using Shell.Common;
using Shell.Windowing;
using TMPro;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Data;
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

		[SerializeField]
		private SoundEffectAsset popupSound = null!;
		
		private IFloatingGui parentWindow;
		private ObservableList<MessageBoxButtonData> buttonsList = new ObservableList<MessageBoxButtonData>();
		private TrixelAudioSource trixelAudio = null!;

		/// <inheritdoc />
		public bool CanClose => true;

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
		public IWorkspaceDefinition CreateWindowOverlay()
		{
			return this.parentWindow!.CreateWindowOverlay();
		}

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
			this.MustGetComponentInParent(out this.parentWindow);
			this.MustGetComponent(out trixelAudio);
			
			this.buttonsList.ItemAdded += HandleItemAdded;
			this.buttonsList.ItemRemoved += HandleItemRemoved;
			
			parentWindow.EnableMaximizeButton = false;
			parentWindow.EnableMinimizeButton = false;
			parentWindow.EnableCloseButton = false;
			
			parentWindow.MinimumSize = Vector2.zero;
		}

		private void Start()
		{
			trixelAudio.Play(this.popupSound);
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
	}
}