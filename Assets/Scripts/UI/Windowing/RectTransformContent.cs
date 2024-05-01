#nullable enable
using System;
using Cysharp.Threading.Tasks;
using Shell.Windowing;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windowing
{
	public class RectTransformContent : IContent
	{
		private readonly GameObject gameObject;
		private readonly RectTransform rectTransform;
		private IContentPanel? parent;
		private VerticalLayoutGroup layoutGroup;

		/// <inheritdoc />
		public IContentPanel? Parent => parent;
		
		public RectTransform RectTransform => rectTransform;
		
		public RectTransformContent()
		{
			this.gameObject = new GameObject(nameof(RectTransformContent));
			this.rectTransform = this.gameObject.AddComponent<RectTransform>();
			this.layoutGroup = this.gameObject.AddComponent<VerticalLayoutGroup>();

			this.layoutGroup.childControlWidth = true;
			this.layoutGroup.childControlHeight = true;
			this.layoutGroup.childScaleWidth = true;
			this.layoutGroup.childScaleHeight = true;
			
			this.gameObject.SetActive(false);
		}

		/// <inheritdoc />
		public void OnParentChanged(IContentPanel? parent)
		{
			if (this.parent == parent)
				return;

			this.parent = parent;

			if (parent?.Content != null)
				throw new InvalidOperationException("Cannot assign content to a content panel that already has content.");
			
			if (parent is not RectTransformContentPanel rtParent)
			{
				if (parent != null)
					Debug.LogError($"Attempting to use a {nameof(RectTransformContent)} in a content panel that doesn't support rect transforms.");
				
				this.parent = null;
				this.gameObject.SetActive(false);
				this.gameObject.transform.SetParent(null);
				return;
			}
		
			this.rectTransform.SetParent(rtParent.RectTransform);
			this.rectTransform.localScale = Vector3.one;
			
			// Depth fix
			Vector3 anchoredPos = rectTransform.anchoredPosition3D;
			anchoredPos.z = 0;
			rectTransform.anchoredPosition3D = anchoredPos;
			
			this.gameObject.SetActive(true);
			this.gameObject.layer = rtParent.RectTransform.gameObject.layer;
		}
	}
}