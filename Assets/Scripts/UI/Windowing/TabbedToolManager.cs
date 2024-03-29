#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Architecture;
using GamePlatform;
using OS.Devices;
using Shell;
using Shell.Windowing;
using UI.Applications.WebBrowser;
using UI.Shell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace UI.Windowing
{
	public class TabbedToolManager : UIBehaviour
	{
		[SerializeField]
		private GameManagerHolder gameManager = null!;

		[SerializeField]
		private DockGroup iconGroup = null!;

		[SerializeField]
		private MainToolGroup terminal = null!;

		[SerializeField]
		private MainToolGroup browser = null!;
		
		private Desktop shell;
		private readonly List<TabbedTool> tools = new List<TabbedTool>();
		private TabbedTool? currentTool;
		private ITile tile;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TabbedToolManager));
			this.MustGetComponent(out tile);
			this.MustGetComponentInParent(out shell);
			this.tile.WindowClosed += OnTileClosed;
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			this.tools.Clear();

			if (gameManager.Value == null)
				return;
			
			// Really cursed bullshit linq code that I'd never get CAUGHT DEAD writing for trixel...
			// that makes the terminal always show up first no matter what.
			this.tools.AddRange(this.gameManager.Value.AvailableTools
				.OrderByDescending(x=>x.Equals(terminal))
				.ThenBy(x=>x.Program.WindowTitle)
				.Select(x=>new TabbedTool(x, shell.LoginProcess)));

			this.BuildDock();

			if (tools.Count == 0)
				return;
			
			this.SwitchTools(tools[0]);
		}
		
		private void BuildDock()
		{
			// We only ever execute this once during the game session, or very rarely if a skill is unlocked that adds a new tool.
			// So we can get away with clearing out the entire tools list.
			this.iconGroup.Clear();
			
			// And now add icons for all tools.
			foreach (TabbedTool tool in tools)
			{
				iconGroup.Add(new DockGroup.IconDefinition()
				{
					ClickHandler = () => SwitchTools(tool),
					Label = tool.Definition.Program.WindowTitle,
					Icon = tool.Definition.Program.Icon,
					IsActive = false
				});
			}
		}

		private void MustGetToolGui<T>(out T behaviour) where T : MonoBehaviour
		{
			if (this.tile.ActiveContent is not RectTransformContentPanel contentPanel)
				throw new InvalidOperationException("The current active tool is not a Unity UI-based tool.");

			contentPanel.RectTransform.MustGetComponentInChildren(out behaviour);
		}
		
		public void OpenWebBrowser(Uri uri)
		{
			if (uri.Scheme != "web")
				throw new InvalidOperationException("You cannot open a URL with this scheme in the Web Browser.");

			this.SwitchTools(browser);

			this.MustGetToolGui(out WebBrowserController webBrowserController);

			webBrowserController.Navigate(uri);
		}
		
		private void UpdateDockActiveStates()
		{
			int activeIndex = -1;
			if (currentTool != null)
				activeIndex = tools.IndexOf(currentTool);
			
			for (var i = 0; i < iconGroup.Count; i++)
			{
				DockGroup.IconDefinition icon = iconGroup[i];

				bool shouldBeActive = i == activeIndex;

				if (icon.IsActive == shouldBeActive)
					continue;

				icon.IsActive = shouldBeActive;

				iconGroup[i] = icon;
			}
		}

		private void DeactivateCurrentTool()
		{
			if (currentTool == null)
				return;

			currentTool.SaveState(this.tile);
			currentTool = null;
			
			// Remove all but one tab (Removing last tab will close the window.)
			while (tile.Tabs.Count > 1)
			{
				this.tile.RemoveTab(tile.Tabs.Last());
			}
			
			UpdateDockActiveStates();
		}

		private void ActivateCurrentTool()
		{
			if (this.currentTool == null)
				return;

			tile.Show();
			this.currentTool.RestoreState(this.tile);
			this.UpdateDockActiveStates();
		}
		
		private void SwitchTools(TabbedTool tool)
		{
			if (tool == this.currentTool)
			{
				DeactivateCurrentTool();

				tile.Hide();
				
				return;
			}

			DeactivateCurrentTool();

			currentTool = tool;

			ActivateCurrentTool();
		}

		public void SwitchTools(string toolId)
		{
			if (this.currentTool != null && this.currentTool.Definition.Program.BinaryName == toolId)
				return;
			
			TabbedTool? tool = tools.FirstOrDefault(x => x.Definition.Program.BinaryName == toolId);
			if (tool == null)
				throw new InvalidOperationException($"Tool not found: {toolId}");
			
			this.SwitchTools(tool);
		}
		
		public void SwitchTools(ITabbedToolDefinition tool)
		{
			if (TryMapPrimaryTool(tool, out TabbedTool mainTool))
			{
				SwitchTools(mainTool);
				return;
			}
			
			// TODO: Support temporary tools
			throw new NotImplementedException();
		}

		private bool TryMapPrimaryTool(ITabbedToolDefinition definition, out TabbedTool? tool)
		{
			tool = this.tools.FirstOrDefault(x => x.Definition == definition);
			return tool != null;
		}
		
		private class TabbedTool
		{
			private readonly List<ISystemProcess> childProcesses = new List<ISystemProcess>();
			private readonly ITabbedToolDefinition definition;
			private readonly List<TabState> savedTabs = new List<TabState>();
			private readonly ISystemProcess parentProcess;
			
			public ITabbedToolDefinition Definition => definition;
			
			public TabbedTool(ITabbedToolDefinition definition, ISystemProcess parentProcess)
			{
				this.definition = definition;
				this.parentProcess = parentProcess;
			}

			public void SaveState(ITile tile)
			{
				this.savedTabs.Clear();

				var i = 0;
				foreach (IContentPanel panel in tile.Tabs)
				{
					savedTabs.Add(new TabState
					{
						Title = panel.Title,
						Content = panel.Content,
						IsActiveTab = tile.ActiveContent == panel,
						Process = childProcesses[i]
					});

					panel.Content = null;
					i++;
				}
			}

			public void RestoreState(ITile tile)
			{
				tile.SetWindowHints(default);
				
				tile.Icon = Definition.Program.Icon;
				tile.ShowNewTab = definition.AllowUserTabs;
				tile.NewTabCallback = () => OpenNewTab(tile);
				
				if (savedTabs.Count == 0)
				{
					this.OpenNewTab(tile);
					return;
				}

				this.childProcesses.Clear();
				
				int activeIndex = -1;
				var i = 0;
				foreach (TabState state in savedTabs)
				{
					if (state.IsActiveTab)
						activeIndex = i;
					
					IContentPanel panel = tile.ActiveContent;
					if (panel.Content != null)
						panel = tile.CreateTab();

					panel.Title = state.Title;
					panel.Content = state.Content;
					
					childProcesses.Add(state.Process);
					
					i++;
				}
				
				if (activeIndex != -1)
					tile.SwitchTab(activeIndex);
				
				savedTabs.Clear();
			}

			public void OpenNewTab(ITile tile)
			{
				ISystemProcess process = parentProcess.Fork();
				this.childProcesses.Add(process);
				
				IContentPanel panel = tile.ActiveContent;
				if (panel.Content != null)
					panel = tile.CreateTab();

				this.Definition.Program.InstantiateIntoWindow(process, panel, null, Array.Empty<string>());
			}
		}

		public struct TabState
		{
			public string Title;
			public bool IsActiveTab;
			public IContent? Content;
			public ISystemProcess Process;
		}

		private void OnTileClosed(IWindow window)
		{
			this.currentTool = null;
			this.UpdateDockActiveStates();
		}
	}
}