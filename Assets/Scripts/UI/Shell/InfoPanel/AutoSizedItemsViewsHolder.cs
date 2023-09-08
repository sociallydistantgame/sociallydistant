#nullable enable
using Com.TheFallenGames.OSA.Core;
using UnityEngine;

namespace UI.Shell.InfoPanel
{
	public abstract class AutoSizedItemsViewsHolder : BaseItemViewsHolder
	{
		/// <inheritdoc />
		public override void MarkForRebuild()
		{
			base.MarkForRebuild();
			root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((RectTransform) root.GetChild(0)).rect.height);
		}
	}
}