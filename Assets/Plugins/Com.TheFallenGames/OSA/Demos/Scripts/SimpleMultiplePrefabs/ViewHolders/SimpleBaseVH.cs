using System;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.Models;

namespace Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.ViewsHolders
{
    /// <summary>
	/// Includes common functionalities for the 3 views holders. <see cref="CanPresentModelType(Type)"/> is 
	/// implemented in all of them to return wether the views holder can present a model of specific type (that's 
	/// why we cache the model's type into <see cref="SimpleBaseModel.CachedType"/> inside its constructor)
	/// </summary>
    public abstract class SimpleBaseVH : BaseItemViewsHolder
    {
        public Text titleText;


		/// <inheritdoc/>
        public override void CollectViews()
        {
            base.CollectViews();

			root.GetComponentAtPath("TitleText", out titleText);
        }

        public abstract bool CanPresentModelType(Type modelType);

		/// <summary>
		/// Called to update the views from the specified model. 
		/// Overriden by inheritors to update their own views after casting the model to its known type.
		/// </summary>
		public virtual void UpdateViews(SimpleBaseModel model)
		{
			if (titleText)
				titleText.text = "#" + ItemIndex;
		}

		/// <summary>Used to manually update the RectTransform's size based on custom rules each VH type specifies</summary>
		public virtual void UpdateSize()
		{
		}
    }
}
