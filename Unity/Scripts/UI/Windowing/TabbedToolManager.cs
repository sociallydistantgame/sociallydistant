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
using System.Threading.Tasks;

namespace UI.Windowing
{
	public class TabbedToolManager : UIBehaviour
	{
		
		private DockGroup iconGroup = null!;

		
		private MainToolGroup terminal = null!;

		
		private MainToolGroup browser = null!;
		
		private Desktop shell;
		private readonly List<TabbedTool> tools = new List<TabbedTool>();
		private TabbedTool? currentTool;
		private ITile tile;
		private GameManager gameManager = null!;

		/// <inheritdoc />
		protected override void Awake()
		{
			gameManager = GameManager.Instance;

			this.AssertAllFieldsAreSerialized(typeof(TabbedToolManager));
			this.MustGetComponent(out tile);
			this.MustGetComponentInParent(out shell);
			this.tile.WindowClosed += OnTileClosed;

			this.tools.Clear();

			// Really cursed bullshit linq code that I'd never get CAUGHT DEAD writing for trixel...
			// that makes the terminal always show up first no matter what.
			this.tools.AddRange(this.gameManager.AvailableTools
				.OrderByDescending(x => x.Equals(terminal))
				.ThenBy(x => x.Program.WindowTitle)
				.Select(x => new TabbedTool(x, shell)));

			this.BuildDock();

			base.Awake();
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
					ClickHandler = async () => await SwitchTools(tool),
					Label = tool.Definition.Program.WindowTitle,
					Icon = tool.Definition.Program.Icon,
					IsActive = false,
					NotificationGroup = tool.Definition.NotificationGroup
				});
			}
		}
		
		private void MustGetToolGui<T>(out T behaviour) where T : MonoBehaviour
		{
			if (this.tile.ActiveContent is not RectTransformContentPanel contentPanel)
				throw new InvalidOperationException("The current active tool is not a Unity UI-based tool.");

			contentPanel.RectTransform.MustGetComponentInChildren(out behaviour);
		}
		
		public async Task OpenWebBrowser(Uri uri)
		{
			if (uri.Scheme != "web")
				throw new InvalidOperationException("You cannot open a URL with this scheme in the Web Browser.");

			await this.SwitchTools(browser);

			this.MustGetToolGui(out WebBrowserController webBrowserController);

			webBrowserController.Navigate(uri);
		}

		public async Task StartFirstTool()
		{
			if (this.tools.Count == 0)
				return;

			await SwitchTools(this.tools.First());
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

		private async Task ActivateCurrentTool()
		{
			if (this.currentTool == null)
				return;

			tile.Show();
			await this.currentTool.RestoreState(this.tile);
			this.UpdateDockActiveStates();
		}
		
		private async Task SwitchTools(TabbedTool tool)
		{
			if (tool == this.currentTool)
			{
				DeactivateCurrentTool();

				tile.Hide();
				
				return;
			}

			DeactivateCurrentTool();

			currentTool = tool;

			await ActivateCurrentTool();
		}

		public async Task SwitchTools(string toolId)
		{
			if (this.currentTool != null && this.currentTool.Definition.Program.BinaryName == toolId)
				return;
			
			TabbedTool? tool = tools.FirstOrDefault(x => x.Definition.Program.BinaryName == toolId);
			if (tool == null)
				throw new InvalidOperationException($"Tool not found: {toolId}");
			
			await this.SwitchTools(tool);
		}
		
		public async Task SwitchTools(ITabbedToolDefinition tool)
		{
			if (TryMapPrimaryTool(tool, out TabbedTool mainTool))
			{
				await SwitchTools(mainTool);
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
			private readonly Desktop shell;
			
			public ITabbedToolDefinition Definition => definition;
			
			public TabbedTool(ITabbedToolDefinition definition, Desktop shell)
			{
				this.definition = definition;
				this.shell = shell;
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

			public async Task RestoreState(ITile tile)
			{
				tile.SetWindowHints(default);
				
				tile.Icon = Definition.Program.Icon;
				tile.ShowNewTab = definition.AllowUserTabs;
				tile.NewTabCallback = async () => await OpenNewTab(tile);
				
				if (savedTabs.Count == 0)
				{
					await this.OpenNewTab(tile);
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

			public async Task OpenNewTab(ITile tile)
			{
				ISystemProcess process = await shell.LoginProcess.Fork();
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