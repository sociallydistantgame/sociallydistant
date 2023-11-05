#nullable enable
using System.Collections.Generic;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.DataHelpers;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Applications.FileManager
{
	public class FileGridAdapter : GridAdapter<GridParams, FileViewsHolder>
	{
		[SerializeField]
		public UnityEvent<string> onFileDoubleClicked = new UnityEvent<string>();
		
		private SimpleDataHelper<ShellFileModel> data;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			data = new SimpleDataHelper<ShellFileModel>(this);
			base.Awake();
		}

		public void SetFiles(IList<ShellFileModel> files)
		{
			if (!IsInitialized)
				Init();
			
			data.ResetItems(files);
		}

		/// <inheritdoc />
		protected override void UpdateCellViewsHolder(FileViewsHolder viewsHolder)
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