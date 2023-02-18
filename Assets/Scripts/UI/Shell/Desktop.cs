#nullable enable

using System;
using Player;
using UI.Windowing;
using UnityEngine;
using Utility;

namespace UI.Shell
{
	public class Desktop :
		MonoBehaviour,
		IDesktop
	{
		private IWorkspaceDefinition currentWorkspace = null!;

		[Header("Dependencies")]
		[SerializeField]
		private PlayerInstanceHolder playerHolder = null!;

		[Header("UI")]
		[SerializeField]
		private RectTransform workspaceArea = null!;
		
		public IWorkspaceDefinition CurrentWorkspace => CurrentWorkspace;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(Desktop));
		}

		private void Start()
		{
			this.currentWorkspace = playerHolder.Value.WindowManager.DefineWorkspace(this.workspaceArea);

			this.currentWorkspace.CreateWindow("desktop window");
		}
	}
}