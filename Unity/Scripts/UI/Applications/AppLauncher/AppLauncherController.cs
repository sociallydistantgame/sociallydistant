#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Architecture;
using GamePlatform;
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
		
		private PlayerInstanceHolder playerInstance = null!;
		
		[Header("UI")]
		
		private FileGridAdapter grid = null!;

		private readonly List<UguiProgram> programs = new List<UguiProgram>();
		private readonly Dictionary<string, int> programMap = new Dictionary<string, int>();
		private ISystemProcess? process;
		private GameManager game = null!;

		/// <inheritdoc />
		protected override void Awake()
		{
			game = GameManager.Instance;
			
			this.AssertAllFieldsAreSerialized(typeof(AppLauncherController));
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			this.programs.Clear();

			this.programs.AddRange(game.ContentManager.GetContentOfType<UguiProgram>());
			
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
			this.process = process;
		}
		
		private async void OnFileDoubleClicked(string file)
		{
			if (!programMap.TryGetValue(file, out int index))
				return;

			UguiProgram program = programs[index];

			if (playerInstance.Value.UiManager.Desktop == null)
				return;
			
			await playerInstance.Value.UiManager.Desktop.OpenProgram(program, Array.Empty<string>(), null, null);

			process?.Kill();
		}
	}
}