#nullable enable
using System;
using UI.ScrollViews;
using UI.Shell.InfoPanel;
using UnityExtensions;

namespace UI.Applications.FileManager
{
	public class FileViewsHolder : AutoSizedItemsViewsHolder
	{
		private ShellFileView view;

		public Action<string>? Callback { get; set; }
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out view);
			base.CollectViews();
		}

		public void UpdateData(ShellFileModel fileData)
		{
			view.Callback = Callback;
			view.UpdateData(fileData);
		}
	}
}