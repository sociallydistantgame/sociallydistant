#nullable enable

using System.Collections.Generic;
using Architecture;
using UnityEngine;
using Utility;

namespace UI.Windowing
{
	public class WindowManager : MonoBehaviour, IWindowManager<UguiWorkspaceDefinition, RectTransform, RectTransform>
	{
		private UguiWorkspaceDefinition fallbackWorkspace = null!;
		
		[Header("Configuration")]
		[SerializeField]
		private RectTransform fallbackWorkspaceArea = null!;
		
		[Header("Prefabs")]
		[SerializeField]
		private UguiWindow windowPrefab = null!;
		
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
	}
}