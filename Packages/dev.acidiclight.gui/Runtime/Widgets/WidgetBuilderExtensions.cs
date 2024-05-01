using System;

namespace AcidicGui.Widgets
{
	public static class WidgetBuilderExtensions
	{
		public static WidgetBuilder AddLabel(this WidgetBuilder builder, string labelText, SectionWidget? section = null)
		{
			return builder.AddWidget(new LabelWidget
			{
				Text = labelText
			}, section);
		}

		public static WidgetBuilder AddButton(this WidgetBuilder builder, string text, Action? clickCallback, SectionWidget? section = null)
		{
			return builder.AddWidget(new ButtonWidget
			{
				Text = text,
				ClickAction = clickCallback
			}, section);
		}
	}
}