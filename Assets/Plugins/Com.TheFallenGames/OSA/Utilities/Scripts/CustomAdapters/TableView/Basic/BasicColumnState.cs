using System;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Basic
{
	public class BasicColumnState : IColumnState
	{
		public IColumnInfo Info { get; private set; }
		public TableValueSortType CurrentSortingType { get; set; }
		public bool CurrentlyReadOnly { get; set; }
		public float CurrentSize { get; set; }


		public BasicColumnState(IColumnInfo info, bool readonlyByDefault)
		{
			Info = info;
			CurrentSortingType = TableValueSortType.NONE;
			CurrentlyReadOnly = readonlyByDefault;
			CurrentSize = info.Size;
		}
	}
}