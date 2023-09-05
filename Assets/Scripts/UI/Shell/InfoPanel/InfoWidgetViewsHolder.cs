#nullable enable
using Com.TheFallenGames.OSA.Core;
using Shell.InfoPanel;
using UnityExtensions;
using Utility;

namespace UI.Shell.InfoPanel
{
	public class InfoWidgetViewsHolder : BaseItemViewsHolder
	{
		private InfoWidgetController view;
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponent(out view);
			base.CollectViews();
		}

		public void SetInfo(InfoWidgetData data)
		{
			view.SetData(data);
		}
	}
}