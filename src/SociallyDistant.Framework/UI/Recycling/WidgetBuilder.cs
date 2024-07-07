using AcidicGUI.ListAdapters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SociallyDistant.Core.UI.Recycling;

public class WidgetBuilder
{
	private readonly Dictionary<SectionWidget, List<IWidget>> sectionedWidgets     = new Dictionary<SectionWidget, List<IWidget>>();
	private readonly List<IWidget>                            uncategorizedWidgets = new List<IWidget>();
	private readonly Stack<SectionWidget>                     sectionStack         = new Stack<SectionWidget>();
	private          bool                                     isBuilding           = false;

	public string CurrentSectionName
	{
		get
		{
			if (sectionStack.Count == 0)
				return string.Empty;

			return sectionStack.Peek().Text;
		}
	}

	public string? Name { get; set; }
	public string? Description { get; set; }

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

		section = new SectionWidget { Text = title };

		this.sectionedWidgets.Add(section, new List<IWidget>());
		return this;
	}

	public WidgetBuilder PushDefaultSection(string sectionTitle, out SectionWidget widget)
	{
		AddSection(sectionTitle, out widget);

		sectionStack.Push(widget);

		return this;
	}

	public WidgetBuilder PopDefaultSection()
	{
		ThrowIfNotBuilding();

		sectionStack.Pop();

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
			if (sectionStack.Count == 0)
			{
				uncategorizedWidgets.Add(widget);
				return this;
			}
			else
			{
				section = sectionStack.Peek();
			}
		}

		if (!sectionedWidgets.TryGetValue(section, out List<IWidget> widgetList))
		{
			widgetList = new List<IWidget>();
			sectionedWidgets.Add(section, widgetList);
		}

		widgetList.Add(widget);

		return this;
	}

	public WidgetBuilder AddImage(Texture2D texture, Color? color = null, SectionWidget? section = null)
	{
		return AddWidget(new ImageWidget { Texture = texture, Color = color }, section);
	}

	public IList<RecyclableWidgetController> Build()
	{
		ThrowIfNotBuilding();

		var result = new List<RecyclableWidgetController>();

		if (ShowUncategorizedFirst)
		{
			result.AddRange(uncategorizedWidgets.Select(x=>x.Build()));
		}

		foreach (SectionWidget section in sectionedWidgets.Keys)
		{
			if (SkipEmptySections && sectionedWidgets[section].Count == 0)
				continue;

			result.Add(section.Build());

			result.AddRange(sectionedWidgets[section].Select(x=>x.Build()));
		}

		if (!ShowUncategorizedFirst)
		{
			result.AddRange(uncategorizedWidgets.Select(x=>x.Build()));
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