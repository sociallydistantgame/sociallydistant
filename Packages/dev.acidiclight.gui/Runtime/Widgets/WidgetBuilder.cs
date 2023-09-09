using System;
using System.Collections.Generic;
using System.Linq;

namespace AcidicGui.Widgets
{
	public class WidgetBuilder
	{
		private readonly Dictionary<SectionWidget, List<IWidget>> sectionedWidgets = new Dictionary<SectionWidget, List<IWidget>>();
		private readonly List<IWidget> uncategorizedWidgets = new List<IWidget>();
		private bool isBuilding = false;

		public bool ShowUncategorizedFirst { get; set; } = true;
		public bool SkipEmptySections { get; set; } = true;
		
		public void Begin()
		{
			ThrowIfBuilding();

			this.sectionedWidgets.Clear();
			this.uncategorizedWidgets.Clear();

			isBuilding = true;
		}

		private void ThrowIfBuilding()
		{
			if (isBuilding)
				throw new InvalidOperationException("You can only start a new widget list after finalizing the current list.");
		}

		public WidgetBuilder AddSection(string title, out SectionWidget section)
		{
			ThrowIfNotBuilding();
			
			section = new SectionWidget
			{
				Text = title
			};
			
			this.sectionedWidgets.Add(section, new List<IWidget>());
			return this;
		}

		public WidgetBuilder AddSection<TSection>(out TSection section)
			where TSection : SectionWidget, new()
		{
			ThrowIfNotBuilding();
			
			section = new TSection();
			
			sectionedWidgets.Add(section, new List<IWidget>());

			return this;
		}

		public WidgetBuilder AddWidget(IWidget widget, SectionWidget? section = null)
		{
			ThrowIfNotBuilding();

			if (widget is SectionWidget otherSection)
			{
				sectionedWidgets.Add(otherSection, new List<IWidget>());
				return this;
			}

			if (section == null)
			{
				uncategorizedWidgets.Add(widget);
				return this;
			}

			if (!sectionedWidgets.TryGetValue(section, out List<IWidget> widgetList))
			{
				widgetList = new List<IWidget>();
				sectionedWidgets.Add(section, widgetList);
			}

			widgetList.Add(widget);

			return this;
		}

		public IList<IWidget> Build()
		{
			ThrowIfNotBuilding();

			var result = new List<IWidget>();

			if (ShowUncategorizedFirst)
			{
				result.AddRange(uncategorizedWidgets);
			}

			foreach (SectionWidget section in sectionedWidgets.Keys)
			{
				if (SkipEmptySections && sectionedWidgets[section].Count == 0)
					continue;
				
				result.Add(section);
				
				result.AddRange(sectionedWidgets[section]);
			}
			
			if (!ShowUncategorizedFirst)
			{
				result.AddRange(uncategorizedWidgets);
			}
			
			isBuilding = false;

			return result;
		}
		
		private void ThrowIfNotBuilding()
		{
			if (!isBuilding)
				throw new InvalidOperationException("Please begin the widget list first.");
		}
	}
}