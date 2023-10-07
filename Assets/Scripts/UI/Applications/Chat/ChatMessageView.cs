#nullable enable
using System;
using System.Collections.Generic;
using AcidicGui.Widgets;
using Core.WorldData.Data;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public class ChatMessageView : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField]
		private StaticWidgetList documentArea = null!;

		[SerializeField]
		private RawImage avatar = null!;

		[SerializeField]
		private RectTransform bubbleSpacer = null!;

		[SerializeField]
		private VerticalLayoutGroup documentLayout = null!;

		[SerializeField]
		private HorizontalLayoutGroup messageLayout = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ChatMessageView));
		}

		public void UpdateMessage(ChatMessageModel model)
		{
			// Hide/show bubble spacer based on whether we're a bubble message
			bubbleSpacer.gameObject.SetActive(model.UseBubbleStyle);
			
			// Use reverse layout if we're a bubble message from the player.
			messageLayout.reverseArrangement = model.UseBubbleStyle && model.IsFromPlayer;
			
			// Adjust document element alignment based on the above.
			documentLayout.childAlignment = messageLayout.reverseArrangement ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
			
			// Set avatar!
			avatar.texture = model.Avatar;
			
			// Rebuild document
			RebuildDocument(model.DisplayName, model.Username, model.FormattedDateTime, model.UseBubbleStyle, model.IsFromPlayer, model.DocumentData);
		}

		private void RebuildDocument(string displayName, string username, string formattedDate, bool bubble, bool isPlayer, IEnumerable<DocumentElement> elements)
		{
			var builder = new WidgetBuilder();

			builder.Begin();

			if (!bubble)
			{
				// TODO: Allow display name and date to be shown in the label of bubbles for a11y.
				builder.AddWidget(new LabelWidget
				{
					Text = $"<b>{displayName}</b> <alpha=#80>{formattedDate}<alpha=#FF>"
				});
			}

			foreach (DocumentElement element in elements)
			{
				BuildElement(builder, element, bubble, isPlayer);
			}
			
			this.documentArea.UpdateWidgetList(builder.Build());
		}

		private void BuildElement(WidgetBuilder builder, DocumentElement element, bool bubble, bool isPlayer)
		{
			switch (element.ElementType)
			{
				case DocumentElementType.Text:
				{
					if (bubble)
					{
						builder.AddWidget(new ChatBubbleWidget
						{
							Text = element.Data,
							UsePlayerColor = isPlayer
						});
					}
					else
					{
						builder.AddWidget(new LabelWidget
						{
							Text = element.Data
						});
					}
					break;
				}
				case DocumentElementType.Image:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}