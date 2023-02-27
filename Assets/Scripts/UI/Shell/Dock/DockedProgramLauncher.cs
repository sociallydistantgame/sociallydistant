#nullable enable

using System;
using Architecture;
using Player;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Shell.Dock
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(CompositeIconWidget))]
	[RequireComponent(typeof(Button))]
	public class DockedProgramLauncher : MonoBehaviour
	{
		private CompositeIconWidget iconWidget = null!;
		private Button button;

		[SerializeField]
		private PlayerInstanceHolder player = null!;
		
		[SerializeField]
		private UguiProgram program = null!;

		[SerializeField]
		private string[] arguments = Array.Empty<string>();
		
		private void Awake()
		{
			this.MustGetComponent(out iconWidget);
			this.MustGetComponent(out button);
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

			if (player.Value.Desktop == null)
				return;

			if (program == null)
				return;

			player.Value.Desktop.OpenProgram(program, arguments, null, null);
		}
	}
}