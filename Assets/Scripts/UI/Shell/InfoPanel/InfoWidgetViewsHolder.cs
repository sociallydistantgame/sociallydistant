#nullable enable
using Shell.InfoPanel;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Shell.InfoPanel
{
	public class InfoWidgetViewsHolder : AutoSizedItemsViewsHolder
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