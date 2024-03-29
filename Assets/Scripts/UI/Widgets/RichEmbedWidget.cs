#nullable enable
using AcidicGui.Widgets;
using Shell.InfoPanel;
using UnityEngine;

namespace UI.Widgets
{
	public sealed class RichEmbedWidget : IWidget
	{
		public RichEmbedData EmbedData { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			RichEmbedWidgetController controller = (assembler as SystemWidgets)!.GetRichEmbed(destination);

			controller.EmbedData = EmbedData;
			
			return controller;
		}
	}
}