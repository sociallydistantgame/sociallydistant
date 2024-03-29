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

		[SerializeField]
		private UguiOverlay overlayPrefab = null!;
		
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
			UguiOverlay? overlayGui = Instantiate(overlayPrefab, transform.parent);
			
			UguiWorkspaceDefinition workspace = DefineWorkspace(overlayGui.RectTransform);
			
			return new OverlayWorkspace(overlayGui, workspace);
		}
		
		/// <inheritdoc />
		public IMessageDialog CreateMessageDialog(string title, IWindow? parent = null)
		{
			IWorkspaceDefinition targetWorkspace = parent?.CreateWindowOverlay() ?? CreateSystemOverlay();
			IFloatingGui messageWindow = targetWorkspace.CreateFloatingGui(title);
			
			// Create a RectTransformContent object to hold the dialog's UI inside.
			// The reason this is confusing and complicated is because much of the windowing API is in the modding SDK,
			// and has no awareness of the fact we're using Unity GUI. Therefore rect transforms are completely opaque to it.
			var rectHolder = new RectTransformContent();
			
			// Assign it to the window.
			messageWindow.ActiveContent.Content = rectHolder;
			
			// instantiate the dialog prefab as disabled
			messageDialogPrefab.gameObject.SetActive(false);
			UguiMessageDialog dialogInstance = Instantiate(messageDialogPrefab, rectHolder.RectTransform);
			messageDialogPrefab.gameObject.SetActive(true);
			
			// wake it up
			dialogInstance.gameObject.SetActive(true);
			return dialogInstance;
		}
	}
}