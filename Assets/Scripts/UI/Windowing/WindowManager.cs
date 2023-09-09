#nullable enable

using System.Collections.Generic;
using Architecture;
using Shell.Windowing;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Windowing
{
	public class WindowManager : 
		MonoBehaviour, 
		IWindowManager<UguiWorkspaceDefinition, RectTransform>
	{
		private UguiWorkspaceDefinition fallbackWorkspace = null!;
		private ObservableList<UguiWorkspaceDefinition> workspaces = new ObservableList<UguiWorkspaceDefinition>();
		
		[Header("Configuration")]
		[SerializeField]
		private RectTransform fallbackWorkspaceArea = null!;
		
		[Header("Prefabs")]
		[SerializeField]
		private UguiWindow windowPrefab = null!;

		[SerializeField]
		private UguiMessageDialog messageDialogPrefab = null!;

		/// <inheritdoc />
		public IReadOnlyList<UguiWorkspaceDefinition> WorkspaceList => workspaces;

		public UguiWorkspaceDefinition FallbackWorkspace => fallbackWorkspace;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(WindowManager));
			this.fallbackWorkspace = DefineWorkspace(fallbackWorkspaceArea);
		}

		/// <inheritdoc />
		public UguiWorkspaceDefinition DefineWorkspace(RectTransform? creationParams)
		{
			if (creationParams == null)
				creationParams = fallbackWorkspaceArea;

			var workspace = new UguiWorkspaceDefinition(creationParams, windowPrefab);
			this.workspaces.Add(workspace);

			return workspace;
		}

		public OverlayWorkspace CreateSystemOverlay()
		{
			var go = new GameObject("Overlay workspace");

			var rt = go.AddComponent<RectTransform>();

			rt.SetParent(this.transform);
			
			rt.anchorMin = new Vector2(0, 0);
			rt.anchorMax = new Vector2(1, 1);
			rt.pivot = Vector2.zero;
			rt.sizeDelta = Vector2.zero;
			rt.anchoredPosition = Vector3.zero;
			rt.localScale = Vector3.one;

			go.AddComponent<CanvasRenderer>();

			var image = go.AddComponent<Image>();

			image.color = new Color(0, 0, 0, 0.75f);

			UguiWorkspaceDefinition workspace = DefineWorkspace(rt);

			return new OverlayWorkspace(go, workspace);
		}
		
		/// <inheritdoc />
		public IMessageDialog CreateMessageDialog(string title, IWindow? parent = null)
		{
			IWorkspaceDefinition targetWorkspace = parent?.Workspace ?? fallbackWorkspace;
			IWindow dialogWindow = targetWorkspace.CreateWindow(title);
			
			// instantiate the dialog prefab as disabled
			messageDialogPrefab.gameObject.SetActive(false);
			UguiMessageDialog dialogInstance = Instantiate(messageDialogPrefab);
			messageDialogPrefab.gameObject.SetActive(true);
			
			// Link it to the parent window.
			dialogInstance.Setup(dialogWindow);
			
			// wake it up
			dialogInstance.gameObject.SetActive(true);
			return dialogInstance;
		}
	}
}