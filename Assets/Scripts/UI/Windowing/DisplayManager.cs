#nullable enable
using System;
using Architecture;
using OS.Devices;
using Shell;
using UI.Shell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace UI.Windowing
{
	public class DisplayManager : UIBehaviour
	{
		[SerializeField]
		private DockGroup iconGroup = null!;
		
		[SerializeField]
		private RectTransform networkViewer = null!;

		[SerializeField]
		private UguiProgram applicationLauncherProgram = null!;

		private Desktop desktop = null!;
		private ISystemProcess? appLauncherProcess;
		
		private readonly DockGroup.IconDefinition[] definitions = new DockGroup.IconDefinition[]
		{
			new DockGroup.IconDefinition()
			{
				Label = "Network viewer",
				Icon = MaterialIcons.Web
			},
			new DockGroup.IconDefinition()
			{
				Label = "Show desktop",
				Icon = MaterialIcons.Monitor
			},
			new DockGroup.IconDefinition()
			{
				Label = "Applications",
				Icon = MaterialIcons.Apps
			}
		};
		
		public bool ShowNetworkViewer
		{
			get => networkViewer.gameObject.activeSelf;
			set
			{
				if (ShowNetworkViewer == value)
					return;
				
				networkViewer.gameObject.SetActive(value);
				definitions[0].IsActive = ShowNetworkViewer;
				UpdateDock();
			}
		}
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(DisplayManager));
			this.MustGetComponent(out desktop);
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			BuildDock();
			UpdateDock();
		}

		private void BuildDock()
		{
			definitions[0].IsActive = ShowNetworkViewer;
			definitions[0].ClickHandler = ToggleNetworkViewer;
			definitions[2].ClickHandler = OpenApplications;
			
			this.iconGroup.Clear();

			foreach (DockGroup.IconDefinition definition in definitions)
			{
				iconGroup.Add(definition);
			}
		}
		
		private void UpdateDock()
		{
			definitions[2].IsActive = appLauncherProcess != null;
			
			for (var i = 0; i < definitions.Length; i++)
			{
				DockGroup.IconDefinition source = definitions[i];
				DockGroup.IconDefinition destination = iconGroup[i];

				if (source.IsActive == destination.IsActive)
					return;

				iconGroup[i] = source;
			}
		}

		public void ToggleNetworkViewer()
		{
			ShowNetworkViewer = !ShowNetworkViewer;
		}

		public void OpenApplications()
		{
			if (this.appLauncherProcess != null)
			{
				this.appLauncherProcess.Kill();
				this.appLauncherProcess = null;
				return;
			}
			
			appLauncherProcess = desktop.OpenProgram(this.applicationLauncherProgram, Array.Empty<string>(), null, null);
			appLauncherProcess.Killed += OnAppLauncherKilled;
			this.UpdateDock();
		}

		private void OnAppLauncherKilled(ISystemProcess obj)
		{
			if (this.appLauncherProcess != null)
			{
				this.appLauncherProcess.Killed -= OnAppLauncherKilled;
				this.appLauncherProcess = null;
			}

			this.UpdateDock();

		}
	}
}