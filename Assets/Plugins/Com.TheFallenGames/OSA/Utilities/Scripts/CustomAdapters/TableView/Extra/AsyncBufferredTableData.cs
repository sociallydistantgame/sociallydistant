using System;
using System.Collections;
using Com.TheFallenGames.OSA.DataHelpers;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Extra
{
	/// <summary>
	/// Class for loading chunks of data asynchronously into a <see cref="BufferredTableData"/> and notifying a <see cref="TableAdapter{TParams, TTupleViewsHolder, THeaderTupleViewsHolder}"/>
	/// about changes, while staying aware of its lifecycle events like <see cref="TableAdapter{TParams, TTupleViewsHolder, THeaderTupleViewsHolder}.ChangeItemsCount(Core.ItemCountChangeMode, int, int, bool, bool)"/>
	/// and invalidating any existing loading tasks when needed
	/// </summary>
	public class AsyncBufferredTableData<TTuple> : ITupleProvider
		where TTuple : ITuple, new()
	{
		public ITableColumns Columns { get; private set; }
		public int Count { get { return _DataSource.Count; } }
		public bool ColumnClearingSupported { get { return false; } }
		public bool ColumnSortingSupported { get { return false; } }
		public AsyncBufferredDataSource<TTuple> Source { get { return _DataSource; } }

		AsyncBufferredDataSource<TTuple> _DataSource;

		/// <summary>
		/// See <see cref="AsyncBufferredDataSource{T}"/>
		/// </summary>
		public AsyncBufferredTableData(ITableColumns columns, int tuplesCount, int chunkBufferSize, AsyncBufferredDataSource<TTuple>.Loader loader)
		{
			_DataSource = new AsyncBufferredDataSource<TTuple>(tuplesCount, chunkBufferSize, loader);

			Columns = columns;
		}


		public ITuple GetTuple(int index) { return _DataSource.GetValue(index); }

		//TTuple CreateEmptyTuple()
		//{
		//	return TableViewUtil.CreateTupleWithEmptyValues<TTuple>(Columns.ColumnsCount);
		//}

		public bool ChangeColumnSortType(int columnIndex, TableValueType columnType, TableValueSortType currentSorting, TableValueSortType nextSorting)
		{ throw new NotSupportedException(); }

		public void SetAllValuesOnColumn(int columnIndex, object sameColumnValueInAllTuples)
		{ throw new NotSupportedException(); }
	}
}