using SociallyDistant.Core.Core.Config;
using SociallyDistant.Core.Core.Systems;

namespace SociallyDistant.Core.UI.Recycling.SettingsWidgets;

public class WidgetListSettingsUiBuilder : ISettingsUiBuilder
{
	private readonly WidgetBuilder                  widgets;
	private readonly UniqueIntGenerator             idGenerator = new UniqueIntGenerator();
	private readonly Dictionary<int, SectionWidget> sectionMap  = new Dictionary<int, SectionWidget>();

	public WidgetListSettingsUiBuilder(WidgetBuilder builder)
	{
		this.widgets = builder;
	}

	/// <inheritdoc />
	public ISettingsUiBuilder AddSection(string sectionTitle, out int sectionId)
	{
		sectionId = idGenerator.GetNextValue();

		widgets.AddSection(sectionTitle, out SectionWidget section);

		sectionMap.Add(sectionId, section);

		return this;
	}

	/// <inheritdoc />
	public ISettingsUiBuilder WithLabel(string labelText, int sectionId)
	{
		widgets.AddWidget(new LabelWidget { Text = labelText }, sectionMap[sectionId]);

		return this;
	}

	/// <inheritdoc />
	public ISettingsUiBuilder WithToggle(
		string title,
		string? description,
		bool value,
		Action<bool> changeCallback,
		int sectionId
	)
	{
		this.widgets.AddWidget(new SettingsFieldWidget { Title = title, Description = description, Slot = new SwitchWidget { IsActive = value, Callback = changeCallback }, UseReverseLayout = false }, sectionMap[sectionId]);

		return this;
	}

	/// <inheritdoc />
	public ISettingsUiBuilder WithSlider(
		string title,
		string? description,
		float value,
		float minimum,
		float maximum,
		Action<float> changeCallback,
		int sectionId
	)
	{
		widgets.AddWidget(new SettingsFieldWidget() { Title = title, Description = description, Slot = new SliderWidget { MinimumValue = minimum, MaximumValue = maximum, Value = value, Callback = changeCallback } }, sectionMap[sectionId]);

		return this;
	}

	/// <inheritdoc />
	public ISettingsUiBuilder WithSlider(
		string title,
		string? description,
		float value,
		int minimum,
		int maximum,
		Action<int> changeCallback,
		int sectionId
	)
	{
		widgets.AddWidget(new SettingsFieldWidget() { Title = title, Description = description, Slot = new SliderWidget { MinimumValue = minimum, MaximumValue = maximum, Value = value, Callback = (newValue) => changeCallback?.Invoke((int)newValue) } }, sectionMap[sectionId]);

		return this;
	}

	/// <inheritdoc />
	public ISettingsUiBuilder WithTextField(
		string title,
		string? description,
		string? currentValue,
		Action<string?> changeCallback,
		int sectionId
	)
	{
		widgets.AddWidget(new SettingsFieldWidget { Title = title, Description = description, Slot = new InputFieldWidget { Value = currentValue, Callback = changeCallback } }, sectionMap[sectionId]);

		return this;
	}

	/// <inheritdoc />
	public ISettingsUiBuilder WithStringDropdown(
		string title,
		string? description,
		int currentIndex,
		string[] choices,
		Action<int> changeCallback,
		int sectionId
	)
	{
		widgets.AddWidget(new SettingsFieldWidget { Title = title, Description = description, Slot = new DropdownWidget { Choices = choices, CurrentIndex = currentIndex, Callback = changeCallback }, }, sectionMap[sectionId]);

		return this;
	}
}