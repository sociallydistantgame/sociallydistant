using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	/// <summary>
	/// A convenience base class for a table's data used to store information for both the columns and tuples (aka 'rows').
	/// Column sorting is not supported if items count exceeds <see cref="TableViewConst.MAX_TABLE_ENTRIES_FOR_ACCEPTABLE_COLUMN_ITERATION_TIME"/>
	/// </summary>
	public abstract class TableData : ITupleProvider
	{
		public ITableColumns Columns { get; protected set; }
		public abstract int Count { get; }
		public abstract bool ColumnClearingSupported { get; }
		public bool ColumnSortingSupported { get { return _ColumnSortingSupported; } protected set { _ColumnSortingSupported = value; } }

		bool _ColumnSortingSupported;
		Dictionary<TableValueType, IComparer> _MapValueTypeToComparer;


		/// <summary>
		/// <paramref name="rowTuples"/> is the list of all the rows in the table, as tuples implementing <see cref="ITuple"/>
		/// </summary>
		public TableData(ITableColumns columnsProvider, bool columnSortingSupported)
		{
			Init(columnsProvider, _ColumnSortingSupported);
		}

		protected TableData()
		{

		}


		/// <summary>Initialization method provided for inheritors, if needed</summary>
		protected void Init(ITableColumns columns, bool columnSortingSupported)
		{
			Columns = columns;

			int maxCountForSorting = TableViewConst.MAX_TABLE_ENTRIES_FOR_ACCEPTABLE_COLUMN_ITERATION_TIME;
			if (Count > maxCountForSorting)
			{
				if (columnSortingSupported)
				{
					Debug.Log(typeof(TableData).Name +
						": columnSortingSupported is true, but the count exceeds MAX_TABLE_ENTRIES_FOR_ACCEPTABLE_COLUMN_SORTING_TIME " +
						"(" + Count + " > " + maxCountForSorting + "). Setting columnSortingSupported=false");
					columnSortingSupported = false;
				}
			}

			_ColumnSortingSupported = columnSortingSupported;

			if (_ColumnSortingSupported)
			{
				_MapValueTypeToComparer = new Dictionary<TableValueType, IComparer>(9);
				var rawComparerForNull = new ObjectComparerForNull();
				_MapValueTypeToComparer[TableValueType.RAW] = rawComparerForNull;
				_MapValueTypeToComparer[TableValueType.STRING] = StringComparer.OrdinalIgnoreCase;
				_MapValueTypeToComparer[TableValueType.INT] = Comparer<int>.Default;
				_MapValueTypeToComparer[TableValueType.LONG_INT] = Comparer<long>.Default;
				_MapValueTypeToComparer[TableValueType.FLOAT] = Comparer<float>.Default;
				_MapValueTypeToComparer[TableValueType.DOUBLE] = Comparer<double>.Default;
				_MapValueTypeToComparer[TableValueType.ENUMERATION] = new EnumComparerSupportingNull();
				_MapValueTypeToComparer[TableValueType.BOOL] = Comparer<bool>.Default;
				_MapValueTypeToComparer[TableValueType.TEXTURE] = rawComparerForNull;
			}
		}

		#region ITupleProvider
		public abstract ITuple GetTuple(int index);

		/// <summary>Expensive operation, if the table contains a lot of entries</summary>
		public bool ChangeColumnSortType(int columnIndex, TableValueType columnType, TableValueSortType currentSorting, TableValueSortType nextSorting)
		{
			if (!_ColumnSortingSupported)
				throw new InvalidOperationException("Cannot sort this table model because it was constructed with columnSortingSupported = false");

			// No comparer means changing sort type is not possible
			IComparer comparer;
			if (!_MapValueTypeToComparer.TryGetValue(columnType, out comparer))
				return false;

			// Sort them
			if (currentSorting == TableValueSortType.NONE)
			{
				bool asc = nextSorting == TableValueSortType.ASCENDING;
				SortTuplesListIfSupported(new TupleComparerWrapper(comparer, asc, columnIndex));
			}
			else
				// No sorting needed, just reversing the list, which is faster
				ReverseTuplesListIfSupported();

			return true;
		}

		/// <summary>Expensive operation, if the table contains a lot of entries</summary>
		public void SetAllValuesOnColumn(int columnIndex, object sameColumnValueInAllTuples)
		{
			for (int i = 0; i < Count; i++)
			{
				GetTuple(i).SetValue(columnIndex, sameColumnValueInAllTuples);
			}
		}
		#endregion

		protected abstract bool SortTuplesListIfSupported(IComparer comparerToUse);
		protected abstract bool ReverseTuplesListIfSupported();


		protected class ObjectComparerForNull : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null)
				{
					if (y != null)
						return -1;
				}
				else if (y == null)
					return 1;

				return 0;
			}
		}


		protected class EnumComparerSupportingNull : ObjectComparerForNull
		{
			Comparer<Enum> _SystemComparer = Comparer<Enum>.Default;


			public int Compare(Enum x, Enum y)
			{
				if (x == null)
				{
					if (y != null)
						return -1;

					return 0; // both null => equal
				}

				if (y == null)
					return 1;

				return _SystemComparer.Compare(x, y);
			}
		}


		protected class TupleComparerWrapper : IComparer
		{
			IComparer _Comparer;
			readonly int _ColumnIndex;
			int _Sign;


			public TupleComparerWrapper(IComparer comparer, bool asc, int columnIndex)
			{
				_Comparer = comparer;
				_ColumnIndex = columnIndex;
				_Sign = asc ? 1 : -1;
			}


			int IComparer.Compare(object a, object b)
			{
				return _Sign * _Comparer.Compare(
							(a as ITuple).GetValue(_ColumnIndex),
							(b as ITuple).GetValue(_ColumnIndex)
						);
			}
		}
	}
}