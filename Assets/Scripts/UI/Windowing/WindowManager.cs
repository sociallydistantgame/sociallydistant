#nullable enable

using System.Collections.Generic;
using Architecture;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace UI.Windowing
{
	public class WindowManager : MonoBehaviour, IWindowManager<UguiWorkspaceDefinition, RectTransform>
	{
		private UguiWorkspaceDefinition fallbackWorkspace = null!;
		
		[Header("Configuration")]
		[SerializeField]
		private RectTransform fallbackWorkspaceArea = null!;
		
		[Header("Prefabs")]
		[SerializeField]
		private UguiWindow windowPrefab = null!;

		[SerializeField]
		private UguiMessageDialog messageDialogPrefab = null!;
		
		/// <inheritdoc />
		public ObservableList<UguiWorkspaceDefinition> WorkspaceList { get; private set; } = new ObservableList<UguiWorkspaceDefinition>();

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
			this.WorkspaceList.Add(workspace);

			return workspace;
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