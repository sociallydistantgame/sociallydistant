#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Architecture;
using OS.Devices;
using Player;
using Shell.Windowing;
using UI.Applications.FileManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace UI.Applications.AppLauncher
{
	public class AppLauncherController : 
		UIBehaviour,
		IProgramOpenHandler
	{
		[SerializeField]
		private PlayerInstanceHolder playerInstance = null!;
		
		[Header("UI")]
		[SerializeField]
		private FileGridAdapter grid = null!;

		private readonly List<UguiProgram> programs = new List<UguiProgram>();
		private readonly Dictionary<string, int> programMap = new Dictionary<string, int>();
		private IWindow window;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(AppLauncherController));
			this.MustGetComponentInParent(out window);
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			this.programs.Clear();
			this.programs.AddRange(Resources.LoadAll<UguiProgram>("Applications"));

			var i = 0;
			foreach (UguiProgram program in programs)
			{
				programMap[program.BinaryName] = i;
				i++;
			}
			
			this.grid.onFileDoubleClicked.AddListener(OnFileDoubleClicked);

			this.grid.SetFiles(programs.Select(x => new ShellFileModel
			{
				Name = x.WindowTitle,
				Icon = x.Icon,
				Path = x.BinaryName
			}).ToList());
		}
		
		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args)
		{
		}
		
		private void OnFileDoubleClicked(string file)
		{
			if (!programMap.TryGetValue(file, out int index))
				return;

			UguiProgram program = programs[index];

			if (playerInstance.Value.UiManager.Desktop == null)
				return;
			
			playerInstance.Value.UiManager.Desktop.OpenProgram(program, Array.Empty<string>(), null, null);

			window.ForceClose();
		}
	}
}