using System;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.Models;

namespace Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.ViewsHolders
{
	/// <summary>The views holder that can present a <see cref="OrangeModel"/></summary>
	public class OrangeVH : SimpleBaseVH
    {
		public Slider valueSlider;


		/// <inheritdoc/>
		public override void CollectViews()
        {
            base.CollectViews();

			root.GetComponentAtPath("ValueSlider", out valueSlider);
        }

		/// <inheritdoc/>
		public override bool CanPresentModelType(Type modelType)
		{ return modelType == typeof(OrangeModel); }

		/// <inheritdoc/>
		public override void UpdateViews(SimpleBaseModel model)
        {
            base.UpdateViews(model);

            var orangeModel = model as OrangeModel;
			valueSlider.value = orangeModel.value;
        }
    }
}
