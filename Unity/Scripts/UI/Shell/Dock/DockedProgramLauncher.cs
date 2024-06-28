#nullable enable

using System;
using Architecture;
using Player;
using UI.Popovers;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Shell.Dock
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(CompositeIconWidget))]
	[RequireComponent(typeof(Popover))]
	[RequireComponent(typeof(Button))]
	public class DockedProgramLauncher : MonoBehaviour
	{
		private CompositeIconWidget iconWidget = null!;
		private Button button = null!;
		private Popover popover = null!;

		
		private PlayerInstanceHolder player = null!;
		
		
		private UguiProgram program = null!;

		
		private string[] arguments = Array.Empty<string>();
		
		private void Awake()
		{
			this.MustGetComponent(out iconWidget);
			this.MustGetComponent(out button);
			this.MustGetComponent(out popover);
		}

		private void OnEnable()
		{
			button.onClick.AddListener(OnClick);
		}

		private void OnDisable()
		{
			button.onClick.RemoveListener(OnClick);
		}

		private void Update()
		{
			if (program == null)
				popover.enabled = false;
			else
			{
				if (!popover.enabled)
					popover.enabled = true;

				popover.Text = program.WindowTitle;
			}
			
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
				return;


			if (program == null)
				iconWidget.Icon = default;
			else
				iconWidget.Icon = program.Icon;
#endif

		}

		private void OnClick()
		{
			if (player == null)
				return;

			if (player.Value.UiManager.Desktop == null)
				return;

			if (program == null)
				return;

			player.Value.UiManager.Desktop.OpenProgram(program, arguments, null, null);
		}
	}
}