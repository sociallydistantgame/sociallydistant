#nullable enable
using System;
using Shell.Common;
using Shell.Windowing;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Windowing
{
	public class RectTransformContentPanel : 
		UIBehaviour, 
		IContentPanel
	{
		private RectTransform rectTransform;
		private IContent? currentContent;
		private VerticalLayoutGroup layoutGroup;

		public RectTransform RectTransform => rectTransform;
		
		/// <inheritdoc />
		public bool CanClose { get; }

		/// <inheritdoc />
		public void Close()
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public void ForceClose()
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public IWindow Window { get; }

		/// <inheritdoc />
		public string Title { get; set; } = "Window title";

		/// <inheritdoc />
		public CompositeIcon Icon { get; set; }

		/// <inheritdoc />
		public IContent? Content
		{
			get => currentContent;
			set
			{
				if (currentContent == value)
					return;

				if (currentContent != null)
					currentContent.OnParentChanged(null);

				if (value != null)
					value.OnParentChanged(this);

				this.currentContent = value;
			}
		}

		/// <inheritdoc />
		public IObservable<string> TitleObservable => this.ObserveEveryValueChanged(x => x.Title);

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();

			this.MustGetComponent(out rectTransform);

			if (!this.TryGetComponent(out layoutGroup))
			{
				this.layoutGroup = this.gameObject.AddComponent<VerticalLayoutGroup>();

				this.layoutGroup.childControlWidth = true;
				this.layoutGroup.childControlHeight = true;
				this.layoutGroup.childScaleWidth = true;
				this.layoutGroup.childScaleHeight = true;
			}
		}
	}
}