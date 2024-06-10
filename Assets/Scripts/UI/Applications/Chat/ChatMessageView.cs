#nullable enable
using System;
using System.Collections.Generic;
using AcidicGui.Widgets;
using Core.WorldData.Data;
using Social;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Core;
using TrixelCreative.TrixelAudio.Data;
using UI.Widgets;
using UI.Widgets.Settings;
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
		private CanvasGroup canvasGroup = null!;
		
		[SerializeField]
		private AvatarWidgetController avatar = null!;

		[SerializeField]
		private RectTransform bubbleSpacer = null!;

		[SerializeField]
		private VerticalLayoutGroup documentLayout = null!;

		[SerializeField]
		private HorizontalLayoutGroup messageLayout = null!;
		
		private IList<IWidget>? builtWidgets;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ChatMessageView));
		}

		private void OnEnable()
		{
			if (builtWidgets != null)
				this.documentArea.UpdateWidgetList(builtWidgets);
		}

		public void UpdateMessage(ChatMessageModel model)
		{
			// Hide/show bubble spacer based on whether we're a bubble message
			bubbleSpacer.gameObject.SetActive(model.UseBubbleStyle);
			
			// Use reverse layout if we're a bubble message from the player.
			messageLayout.reverseArrangement = model.UseBubbleStyle && model.IsFromPlayer;
			
			// Update our pivot point for the "New message" animation
			var rect = (RectTransform) this.transform;
			rect.pivot = messageLayout.reverseArrangement
				? new Vector2(1, 1)
				: new Vector2(0, 1);
			
			// Adjust document element alignment based on the above.
			documentLayout.childAlignment = messageLayout.reverseArrangement ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
			
			// Set avatar!
			avatar.enabled = model.ShowAvatar;
			avatar.AvatarTexture = model.Avatar;
			

			if (model.IsFromPlayer)
			{
				avatar.DefaultAvatarColor = Color.cyan;
			}
			else
			{
				avatar.DefaultAvatarColor = Color.gray;
			}

			avatar.AvatarSize = AvatarSize.Small;
			avatar.UpdateUI();
			
			// Rebuild document
			RebuildDocument(model.DisplayName, model.Username, model.FormattedDateTime, model.UseBubbleStyle, model.IsFromPlayer, model.Document);
			this.documentArea.UpdateWidgetList(builtWidgets);
			
			if (model.IsNewMessage)
			{
				DoMessageAnimation(model.IsFromPlayer);
			}
		}

		private void DoMessageAnimation(bool isFromPlayer)
		{
			RectTransform t = (RectTransform) this.transform;
			t.localScale = Vector3.zero;
			
			canvasGroup.alpha = 0;
			LeanTween.alphaCanvas(canvasGroup, 1, 0.26f);
			LeanTween.scale(t, Vector3.one, 0.26f);
		}
		
		private void RebuildDocument(string displayName, string username, string formattedDate, bool bubble, bool isPlayer, DocumentElement element)
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

			BuildElement(builder, element, bubble, isPlayer);

			builtWidgets = builder.Build();
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