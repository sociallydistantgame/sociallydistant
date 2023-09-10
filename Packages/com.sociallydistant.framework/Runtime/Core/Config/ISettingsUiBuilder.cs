#nullable enable

using System;

namespace Core.Config
{
	public interface ISettingsUiBuilder
	{
		ISettingsUiBuilder AddSection(string sectionTitle, out int sectionId);

		ISettingsUiBuilder WithLabel(string labelText, int sectionId);
		ISettingsUiBuilder WithToggle(string title, string? description, bool value, Action<bool> changeCallback, int sectionId);
		ISettingsUiBuilder WithSlider(string title, string? description, float value, float minimum, float maximum, Action<float> changeCallback, int sectionId);
		ISettingsUiBuilder WithSlider(string title, string? description, float value, int minimum, int maximum, Action<int> changeCallback, int sectionId);
		ISettingsUiBuilder WithTextField(string title, string? description, string? currentValue, Action<string?> changeCallbac, int sectionId);
		ISettingsUiBuilder WithStringDropdown(string title, string? description, int currentIndex, string[] choices, Action<int> changeCallback, int sectionId);
	}
}