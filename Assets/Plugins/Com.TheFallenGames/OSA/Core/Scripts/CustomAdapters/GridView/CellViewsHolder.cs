using System;
using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.CustomAdapters.GridView
{
    /// <summary>
    /// By design, each cell should have exactly one child and it should hold the views. This is because the cell's GameObject must always be active, while the views may not be.
    /// </summary>
    public abstract class CellViewsHolder : AbstractViewsHolder
    {
        /// <summary>The child containing the views, which will be enabled/disabled depending on the layout rules</summary>
        public RectTransform views;

        public LayoutElement rootLayoutElement;


        /// <summary>Calls base's implementation, after which calls <see cref="GetViews"/> whose result is stored in <see cref="views"/></summary>
        public override void CollectViews()
        {
            base.CollectViews();

            views = GetViews();
            if (views == root)
                throw new OSAException("CellViewsHolder: views == root not allowed: you should have a child of root that holds all the views, as the root should always be enabled for layouting purposes");

			rootLayoutElement = root.GetComponent<LayoutElement>();
			if (!rootLayoutElement)
				throw new OSAException("CellViewsHolder: no LayoutElement found on the root: you should add one to configure how the cell's parent LayoutGroup should position/size it");
		}

		/// <inheritdoc/>
		public override void MarkForRebuild()
		{
			base.MarkForRebuild();

			if (views)
				LayoutRebuilder.MarkLayoutForRebuild(views);
		}

		/// <summary>Provide the cell's child GameObject that contains its views</summary>
		protected virtual RectTransform GetViews()
		{
			var viewsTR = root.Find("Views") as RectTransform;
			if (!viewsTR)
				throw new OSAException("Override " + (typeof(CellViewsHolder).Name) + ".GetViews() " +
					"and provide your own path to the child containing the views. For more info, check the Grid example scene");

			return viewsTR;
		}
    }
}
