using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.UI.Windowing;

namespace SociallyDistant.UI;

public sealed class ToolManager
{
	private readonly DockGroup         iconGroup = null!;
	private readonly List<TabbedTool>  tools     = new List<TabbedTool>();
	private readonly DesktopController desktopController;
	private readonly FlexPanel         workRoot      = new();
	private readonly Box               mainToolsArea = new();
	private readonly FloatingWorkspace mainWorkspace = new();
	private          ITile?            tile;

	private TabbedTool? currentTool;

	public Widget RootWidget => workRoot;
	
	internal ToolManager(DesktopController desktopController, DockGroup dockGroup)
	{
		this.iconGroup = dockGroup;
		this.desktopController = desktopController;
		
		workRoot.ChildWidgets.Add(mainToolsArea);

		mainToolsArea.Content = mainWorkspace;

		mainToolsArea.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;

		mainWorkspace.MaximizeAll = true;
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
				IsActive = false,
				NotificationGroup = tool.Definition.NotificationGroup
			});
		}

		this.iconGroup.RefreshDock();
	}
    
	public void StartFirstTool()
	{
		this.tools.Clear();
		this.tools.AddRange(SociallyDistantGame.Instance.AvailableTools
			.Select(x => new TabbedTool(x, desktopController)));
		
		if (this.tools.Count == 0)
			return;

		BuildDock();
		
		SwitchTools(this.tools.First());
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

		iconGroup.RefreshDock();
	}
	
	private void DeactivateCurrentTool()
	{
		if (currentTool == null)
			return;

		currentTool.SaveState(this.tile);
		currentTool = null;
			
		// Remove all but one tab (Removing last tab will close the window.)
		while (tile.ContentPanels.Count > 1)
		{
			this.tile.RemoveTab(tile.ContentPanels.Last());
		}
			
		UpdateDockActiveStates();
	}
	
	private void ActivateCurrentTool()
	{
		if (this.currentTool == null)
			return;

		if (tile == null)
		{
			tile = mainWorkspace.CreateTabbedWindow();
			tile.WindowClosed += OnTileClosed;
		}
		
		tile?.Show();
		this.currentTool.RestoreState(this.tile);
		this.UpdateDockActiveStates();
	}
	
	private void SwitchTools(TabbedTool tool)
	{
		if (tool == this.currentTool)
		{
			DeactivateCurrentTool();

			tile?.Hide();
				
			return;
		}

		DeactivateCurrentTool();

		currentTool = tool;

		ActivateCurrentTool();
	}
	
	public void SwitchTools(string toolId)
	{
		if (this.currentTool != null && this.currentTool.Definition.Program.Name == toolId)
			return;
			
		TabbedTool? tool = tools.FirstOrDefault(x => x.Definition.Program.Name == toolId);
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
	
	private void OnTileClosed(IWindow window)
	{
		this.currentTool = null;
		this.UpdateDockActiveStates();

		this.tile = null;
	}
	
	private class TabbedTool
	{
		private readonly List<ISystemProcess> childProcesses = new List<ISystemProcess>();
		private readonly ITabbedToolDefinition definition;
		private readonly List<TabState> savedTabs = new List<TabState>();
		private readonly DesktopController shell;

		public ITabbedToolDefinition Definition => definition;

		public TabbedTool(ITabbedToolDefinition definition, DesktopController shell)
		{
			this.definition = definition;
			this.shell = shell;
		}

		public void SaveState(ITile tile)
		{
			this.savedTabs.Clear();

			var i = 0;
			foreach (IContentPanel panel in tile.ContentPanels)
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
			tile.NewTabCallback =  () => OpenNewTab(tile);

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
			ISystemProcess process = shell.Fork();
			this.childProcesses.Add(process);

			IContentPanel panel = tile.ActiveContent ?? tile.CreateTab();
			
			if (panel.Content != null)
				panel = tile.CreateTab();

			this.Definition.Program.InstantiateIntoWindow(process, panel, null, Array.Empty<string>());
		}
	}
	
	private struct TabState
	{
		public string Title;
		public bool IsActiveTab;
		public Widget? Content;
		public ISystemProcess Process;
	}
}