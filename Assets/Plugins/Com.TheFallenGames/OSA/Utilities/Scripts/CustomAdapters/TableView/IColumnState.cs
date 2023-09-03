using System;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	public interface IColumnState
	{
		IColumnInfo Info { get; }
		TableValueSortType CurrentSortingType { get; set; }
		bool CurrentlyReadOnly { get; set; }

		/// <summary>
		/// Set to <see cref="IColumnInfo.Size"/> on initialization. Can be changed after.
		/// </summary>
		float CurrentSize { get; set; }
	}
}