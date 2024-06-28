#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Social;
using UnityEngine;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public sealed class TypingIndicator : MonoBehaviour
	{
		
		private RectTransform avatarArea = null!;

		
		private TypingIndicatorAvatar template = null!;

		private readonly List<TypingIndicatorAvatar> avatars = new();
		private CanvasGroup canvasGroup = null!;
		private LTDescr? hideAnim;
		private LTDescr? showAnim;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TypingIndicator));
			this.MustGetComponent(out canvasGroup);
			template.gameObject.SetActive(false);

			canvasGroup.alpha = 0;
		}

		public void UpdateIndicator(IEnumerable<IProfile> typers)
		{
			IProfile[] typerArray = typers.ToArray();

			while (avatars.Count > typerArray.Length)
			{
				Destroy(avatars[^1].gameObject);
				avatars.RemoveAt(avatars.Count - 1);
			}

			for (var i = 0; i < typerArray.Length; i++)
			{
				TypingIndicatorAvatar? avatar = null;
				if (i == avatars.Count)
				{
					avatar = Instantiate(template, this.avatarArea);
					avatar.gameObject.SetActive(true);
					this.avatars.Add(avatar);
				}
				else
				{
					avatar = avatars[i];
				}

				avatar.SetProfile(typerArray[i]);
			}

			if (avatars.Count == 0)
				Hide();
			else
				Show();
		}

		private void Hide()
		{
			if (showAnim != null)
			{
				LeanTween.cancel(showAnim.id);
				showAnim = null;
			}

			if (hideAnim != null)
				return;

			hideAnim = LeanTween.alphaCanvas(canvasGroup, 0, 0.1f)
				.setOnComplete(AfterHide);
		}

		private void Show()
		{
			if (hideAnim != null)
			{
				LeanTween.cancel(hideAnim.id);
				hideAnim = null;
			}

			if (showAnim != null)
				return;

			showAnim = LeanTween.alphaCanvas(canvasGroup, 1, 0.1f)
				.setOnComplete(AfterShow);;
		}

		private void AfterHide()
		{
			hideAnim = null;
		}

		private void AfterShow()
		{
			showAnim = null;
		}
	}
}