using System;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.Models;

namespace Com.TheFallenGames.OSA.Demos.MultiplePrefabs.ViewsHolders
{
    /// <summary>Includes common functionalities for the 2 viewsholders. <see cref="CanPresentModelType(Type)"/> is implemented in both of them to return wether the views holder can present a model of specific type (that's why we cache the model's type into <see cref="BaseModel.CachedType"/> inside its constructor)</summary>
    public abstract class BaseVH : BaseItemViewsHolder
    {
        public Text titleText;

		/// <inheritdoc/>
        public override void CollectViews()
        {
            base.CollectViews();

            //titleText = root.Find("TitleText").GetComponent<Text>();
        }

        public abstract bool CanPresentModelType(Type modelType);

		/// <summary>Called to update the views from the specified model. Overriden by inheritors to update their own views after casting the model to its known type</summary>
		public virtual void UpdateViews(BaseModel model)
        {
			UpdateTitleOnly(model);
		}

		public virtual void UpdateTitleOnly(BaseModel model)
		{
			//titleText.text = model.title;
			titleText.text = "#" + ItemIndex + " [id:" + model.id + "]";
		}
    }
}
