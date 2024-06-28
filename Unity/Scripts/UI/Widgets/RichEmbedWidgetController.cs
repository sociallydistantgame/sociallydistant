#nullable enable
using System;
using AcidicGui.Widgets;
using Shell.InfoPanel;
using UI.Controllers;
using UnityEngine;
using UnityExtensions;

namespace UI.Widgets
{
	public sealed class RichEmbedWidgetController : WidgetController
	{
		
		private InfoBoxController infoBoxController = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(RichEmbedWidgetController));
		}

		public RichEmbedData EmbedData { get; set; }
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			infoBoxController.Title = EmbedData.Title;
			infoBoxController.Text = EmbedData.Content;
			infoBoxController.Color = EmbedData.Color;

			foreach (string buttonName in EmbedData.Actions.Keys)
			{
				infoBoxController.Add(new InfoBoxController.ButtonDefinition
				{
					label = buttonName,
					clickHandler = EmbedData.Actions[buttonName]
				});
			}
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			infoBoxController.Clear();
		}
	}
}