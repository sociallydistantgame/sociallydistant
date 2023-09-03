using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	/// <summary>
	/// This provides tuples (a tuple represents a row in a table)
	/// </summary>
	public interface ITupleProvider
	{
		int Count { get; }
		/// <summary>
		/// Whether the user can clear all values for a column when it clicks it while in <see cref="ITableAdapter.Options"/> has IsClearing true
		/// </summary>
		bool ColumnClearingSupported { get; }
		bool ColumnSortingSupported { get; }
		ITuple GetTuple(int index);
		bool ChangeColumnSortType(int columnIndex, TableValueType columnType, TableValueSortType currentSorting, TableValueSortType nextSorting);
		void SetAllValuesOnColumn(int columnIndex, object sameColumnValueInAllTuples);
	}
}