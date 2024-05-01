#nullable enable
using System;
using System.Collections.Generic;
using Architecture;
using Shell;
using Shell.Common;
using Shell.Windowing;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Core;
using TrixelCreative.TrixelAudio.Data;
using UI.Controllers;
using UnityEngine;
using UnityExtensions;

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

		[SerializeField]
		private SoundEffectAsset errorPopupSound = null!;

		[SerializeField]
		private SoundEffectAsset warningSound = null!;
		
		private MessageBoxType messageType;
		private IFloatingGui parentWindow;
		private ObservableList<MessageBoxButtonData> buttonsList = new ObservableList<MessageBoxButtonData>();
		
		/// <inheritdoc />
		public bool CanClose { get; set; } = true;

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
		public WindowHints Hints => parentWindow.Hints;

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
		public void SetWindowHints(WindowHints hints)
		{
			this.parentWindow.SetWindowHints(hints);
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
		public MessageBoxType MessageType
		{
			get => messageType;
			set => messageType = value;
		}

		/// <inheritdoc />
		public IList<MessageBoxButtonData> Buttons => buttonsList;

		/// <inheritdoc />
		public Action<MessageDialogResult>? DismissCallback { get; set; }
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(UguiMessageDialog));
			this.MustGetComponentInParent(out this.parentWindow);
			
			this.buttonsList.ItemAdded += HandleItemAdded;
			this.buttonsList.ItemRemoved += HandleItemRemoved;
			
			parentWindow.EnableCloseButton = false;
			
			parentWindow.MinimumSize = Vector2.zero;
		}

		private SoundEffectAsset GetMessageSound()
		{
			return this.messageType switch
			{
				MessageBoxType.Error => errorPopupSound,
				MessageBoxType.Warning => warningSound,
				_ => this.popupSound
			};
		}
		
		private void Start()
		{
			AudioManager.PlaySound(GetMessageSound());

			this.infoBoxController.Color = this.MessageType switch
			{
				MessageBoxType.Warning => CommonColor.Yellow,
				MessageBoxType.Error => CommonColor.Red,
				_ => CommonColor.Cyan
			};

			if (this.parentWindow is IColorable colorable)
			{
				colorable.Color = infoBoxController.Color;
			}
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