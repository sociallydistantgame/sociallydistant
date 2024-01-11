#nullable enable
using Shell.Windowing;
using UI.Windowing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace Architecture
{
	public class ProgramContent : 
		UIBehaviour, 
		IContent
	{
		private RectTransformContent content;

		public RectTransform RectTransform => content.RectTransform;
		
		/// <inheritdoc />
		public IContentPanel? Parent => content.Parent;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.content = new RectTransformContent();
			base.Awake();
		}

		/// <inheritdoc />
		public void OnParentChanged(IContentPanel? parent)
		{
			content.OnParentChanged(parent);
			
			
		}
	}
}