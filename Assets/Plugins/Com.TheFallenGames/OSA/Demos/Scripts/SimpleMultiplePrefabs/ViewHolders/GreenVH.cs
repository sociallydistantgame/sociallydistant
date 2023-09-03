using System;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.Models;

namespace Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.ViewsHolders
{
    /// <summary>The views holder that can present an <see cref="GreenModel"/></summary>
    public class GreenVH : SimpleBaseVH
	{
		public Text contentText;


		/// <inheritdoc/>
		public override void CollectViews()
        {
            base.CollectViews();

			root.GetComponentAtPath("ContentText", out contentText);
		}

		/// <inheritdoc/>
		public override bool CanPresentModelType(Type modelType)
		{ return modelType == typeof(GreenModel); }

		/// <inheritdoc/>
		public override void UpdateViews(SimpleBaseModel model)
		{
			base.UpdateViews(model);

            var greenModel = model as GreenModel;
			contentText.text = greenModel.textContent;
        }
    }
}
