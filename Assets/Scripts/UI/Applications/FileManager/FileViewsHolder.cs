#nullable enable
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using UnityEngine;
using UnityExtensions;
using System;

namespace UI.Applications.FileManager
{
	public class FileViewsHolder : CellViewsHolder
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