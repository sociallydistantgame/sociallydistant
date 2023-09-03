using System;
using System.Collections;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Basic
{
	/// <summary>
	/// A table data that's fetched all at once.
	/// </summary>
	public class BasicTableData : TableData
	{
		public override int Count { get { return _RowTuples.Count; } }
		public override bool ColumnClearingSupported { get { return Count < TableViewConst.MAX_TABLE_ENTRIES_FOR_ACCEPTABLE_COLUMN_ITERATION_TIME; } }

		IList _RowTuples;


		/// <summary>
		/// <paramref name="rowTuples"/> is the list of all the rows in the table, as tuples implementing <see cref="ITuple"/>
		/// </summary>
		public BasicTableData(ITableColumns columnsProvider, IList rowTuples, bool columnSortingSupported)
		{
			_RowTuples = rowTuples;
			Init(columnsProvider, columnSortingSupported);
		}

		public override ITuple GetTuple(int index) { return _RowTuples[index] as ITuple; }

		protected override bool ReverseTuplesListIfSupported()
		{
			// The ArrayList.Adapter() is an O(1) operation
			var adapter = ArrayList.Adapter(_RowTuples);
			adapter.Reverse();

			return true;
		}

		protected override bool SortTuplesListIfSupported(IComparer comparerToUse)
		{
			// The ArrayList.Adapter() is an O(1) operation
			var adapter = ArrayList.Adapter(_RowTuples);
			adapter.Sort(comparerToUse);

			return true;
		}
	}
}