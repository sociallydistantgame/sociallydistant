#nullable enable
using System;
using System.Collections.Generic;
using AcidicGui.Widgets;
using Core.WorldData.Data;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Websites.SocialMedia
{
	public class SocialPostController : MonoBehaviour
	{
		[SerializeField]
		private RawImage avatarImage = null!;

		[SerializeField]
		private StaticWidgetList widgetList = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SocialPostController));
		}

		public void SetData(SocialPostModel model)
		{
			this.avatarImage.texture = model.Avatar;
			this.widgetList.UpdateWidgetList(BuildWidgets(model));
		}

		private IList<IWidget> BuildWidgets(SocialPostModel model)
		{
			var builder = new WidgetBuilder();

			builder.Begin();

			// Post header (author)
			builder.AddWidget(new LabelWidget
			{
				Text = $"<b>{model.Name}</b> @{model.Handle}"
			});

			foreach (DocumentElement element in model.Document)
			{
				switch (element.ElementType)
				{
					case DocumentElementType.Text:
						builder.AddWidget(new LabelWidget
						{
							Text = element.Data
						});
						break;
					case DocumentElementType.Image:
						break;
				}
			}
			
			return builder.Build();
		}
	}
}