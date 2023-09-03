using System;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.Models;
using UnityEngine;

namespace Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.ViewsHolders
{
	/// <summary>The views holder that can present an <see cref="AdModel"/>. It exposes a <see cref="Clicked"/> event which is fired when the item is clicked</summary>
	public class AdVH : SimpleBaseVH
    {
		public RawImage adImage;
		public event Action<AdVH> Clicked;


		/// <inheritdoc/>
		public override void CollectViews()
        {
            base.CollectViews();

			root.GetComponentAtPath("AdImage", out adImage);

			var b = root.GetComponent<Button>();
			if (b)
				b.onClick.AddListener(OnAdClicked);
		}

		/// <inheritdoc/>
		public override bool CanPresentModelType(Type modelType)
		{ return modelType == typeof(AdModel); }

		/// <inheritdoc/>
		public override void UpdateViews(SimpleBaseModel model)
        {
			base.UpdateViews(model);

            var adModel = model as AdModel;
			adImage.texture = adModel.adTexture;
		}

		// Rebuild always needed for ads, since they can have different sizes.
		// This can be optimized by managing a "HasPendingSizeChanges" property inside the model, like 
		// the ContentSizeFitter example does to prevent unnecessary rebuilds, but it's not essential for our simple case here
		/// <inheritdoc/>
		public override void UpdateSize()
		{
			float width = adImage.texture.width;
			float height = adImage.texture.height;
			float ratio = 1f;
			if (width > 0f && height > 0f)
				ratio = width / height;

			// Manually setting the item's size to match the aspect ratio of the image it contains
			float heightToSet = root.rect.width / ratio;
			root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightToSet);
		}

		void OnAdClicked()
		{
			if (Clicked != null)
				Clicked(this);
		}
    }
}
