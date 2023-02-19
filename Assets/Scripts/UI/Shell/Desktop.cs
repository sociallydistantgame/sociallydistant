#nullable enable

using System;
using OS.Devices;
using Player;
using UI.Windowing;
using UnityEngine;
using Utility;

namespace UI.Shell
{
	public class Desktop :
		MonoBehaviour,
		IProgramOpener<RectTransform>,
		IDesktop
	{
		private IWorkspaceDefinition currentWorkspace = null!;
		private ISystemProcess loginProcess = null!;
		private IUser loginUser = null!;

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
			this.loginUser = this.playerHolder.Value.Computer.PlayerUser;
			this.loginProcess = this.playerHolder.Value.OsInitProcess.CreateLoginProcess(this.loginUser);
			this.currentWorkspace = playerHolder.Value.WindowManager.DefineWorkspace(this.workspaceArea);
			this.currentWorkspace.CreateWindow("desktop window");
		}

		/// <inheritdoc />
		public ISystemProcess OpenProgram(IProgram<RectTransform> program, string[] arguments)
		{
			throw new NotImplementedException();
		}
	}
}