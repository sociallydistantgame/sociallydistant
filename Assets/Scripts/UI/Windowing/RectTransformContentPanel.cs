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
		private IWindow parentWindow;

		public RectTransform RectTransform => rectTransform;
		
		/// <inheritdoc />
		public bool CanClose { get; set; }

		/// <inheritdoc />
		public void Close()
		{
			if (!CanClose)
				return;

			ForceClose();
		}

		/// <inheritdoc />
		public void ForceClose()
		{
			ITabbedContent? tabbedContent = GetComponentInParent<ITabbedContent>();
			if (tabbedContent == null)
				return;

			tabbedContent.RemoveTab(this);
		}

		/// <inheritdoc />
		public IWindow Window => parentWindow;

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
				this.ApplyHints();
			}
		}

		/// <inheritdoc />
		public IObservable<string> TitleObservable => this.ObserveEveryValueChanged(x => x.Title);

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.MustGetComponentInParent(out parentWindow);
			
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

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			// Depth fix
			Vector3 anchoredPos = rectTransform.anchoredPosition3D;
			anchoredPos.z = 0;
			rectTransform.anchoredPosition3D = anchoredPos;
			
			ApplyHints();
		}

		private void ApplyHints()
		{
			if (this.currentContent == null)
				return;

			if (this.currentContent is not RectTransformContent rtContent)
				return;

			var hintProvider = rtContent.RectTransform.GetComponentInChildren<WindowHintProvider>(true);
			if (hintProvider == null)
				return;
			
			parentWindow.SetWindowHints(hintProvider.Hints);
		}
	}
}