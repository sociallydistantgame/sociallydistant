using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace AcidicGui.Widgets
{
	public abstract class LinkHelper : 
		UIBehaviour,
		IPointerDownHandler,
		IPointerUpHandler
	{
		
		private TextMeshProUGUI text = null!;
		
		
		private bool allowHttpLinks = false;

		private Canvas rootCanvas = null!;
		
		public bool AllowHttpLinks
		{
			get => allowHttpLinks;
			set => allowHttpLinks = value;
		}

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(LinkHelper));
			this.MustGetComponentInParent(out Canvas canvas);
			this.rootCanvas = canvas.rootCanvas;
			base.Awake();
		}

		/// <inheritdoc />
		public void OnPointerUp(PointerEventData eventData)
		{
			Vector2 mousePosition =  eventData.position;

			int linkIndex = TMP_TextUtilities.FindIntersectingLink(this.text, mousePosition, this.rootCanvas.worldCamera);
			if (linkIndex == -1)
				return;

			TMP_LinkInfo link = text.textInfo.linkInfo[linkIndex];

			OnLinkClickedRaw(eventData.button, link);
		}

		protected virtual void OnLinkClickedRaw(PointerEventData.InputButton button, TMP_LinkInfo linkInfo)
		{
			string linkId = linkInfo.GetLinkID();

			if (Uri.TryCreate(linkId, UriKind.Absolute, out Uri uri))
			{
				OnUriClicked(button, uri);
			}
		}

		protected virtual void OnUriClicked(PointerEventData.InputButton button, Uri uri)
		{
			if (AllowHttpLinks && (uri.Scheme == "http" || uri.Scheme == "https"))
			{
				System.Diagnostics.Process.Start(uri.ToString());
				return;
			}
		}

		/// <inheritdoc />
		public void OnPointerDown(PointerEventData eventData)
		{
			// stub
		}
	}
}