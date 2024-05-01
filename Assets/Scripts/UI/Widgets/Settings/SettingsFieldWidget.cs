#nullable enable

using System;
using AcidicGui.Widgets;
using Shell.Common;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public sealed class SettingsFieldWidget : IWidget
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public CompositeIcon Icon { get; set; }
		public bool UseReverseLayout { get; set; }
		public IWidget? Slot { get; set; }


		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			SettingsFieldController controller = ((SystemWidgets) assembler).GetSettingsField(destination);

			controller.Title = Title;
			controller.Description = Description;
			controller.Icon = Icon;
			controller.UseReverseLayout = UseReverseLayout;
			
			if (Slot!=null)
				controller.SlotWidget = Slot.Build(assembler, controller.Slot);

			return controller;
		}
	}
}