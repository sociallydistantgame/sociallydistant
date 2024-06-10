#nullable enable
using System;
using UI.ScrollViews;
using UnityExtensions;

namespace UI.Applications.FileManager
{
	public class FileViewsHolder : GridCellModel
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

		/// <inheritdoc />
		public FileViewsHolder(int itemIndex) : base(itemIndex)
		{ }
	}
}