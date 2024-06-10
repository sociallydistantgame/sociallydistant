#nullable enable
using System.Collections.Generic;
using UI.ScrollViews;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Applications.FileManager
{
	public class FileGridAdapter : GridController<FileViewsHolder>
	{
		[SerializeField]
		public UnityEvent<string> onFileDoubleClicked = new UnityEvent<string>();
		
		private ScrollViewItemList<ShellFileModel> data;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			data = new ScrollViewItemList<ShellFileModel>(this);
			base.Awake();
		}

		public void SetFiles(IList<ShellFileModel> files)
		{
			data.SetItems(files);
		}

		/// <inheritdoc />
		protected override FileViewsHolder CreateModel(int itemIndex)
		{
			var vh = new FileViewsHolder(itemIndex);
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(FileViewsHolder viewsHolder)
		{
			ShellFileModel model = data[viewsHolder.ItemIndex];

			viewsHolder.Callback = OnDoubleClick;
			
			viewsHolder.UpdateData(model);
		}

		private void OnDoubleClick(string path)
		{
			onFileDoubleClicked.Invoke(path);
		}
	}
}