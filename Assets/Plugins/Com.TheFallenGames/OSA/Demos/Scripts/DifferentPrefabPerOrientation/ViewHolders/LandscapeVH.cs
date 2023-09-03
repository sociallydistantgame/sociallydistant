using System;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Demos.DifferentPrefabPerOrientation.Models;

namespace Com.TheFallenGames.OSA.Demos.DifferentPrefabPerOrientation.ViewsHolders
{
	/// <summary>See <see cref="BaseVH"/></summary>
	public class LandscapeVH : BaseVH
    {
        public Text contentText;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			root.GetComponentAtPath("ContentPanel/ContentText", out contentText);
		}

		public override void UpdateViews(CommonModel model)
		{
			base.UpdateViews(model);

			if (contentText)
				contentText.text = model.Content;
		}
	}
}
