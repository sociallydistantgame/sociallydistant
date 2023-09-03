using System;
using System.Collections;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Basic
{
	public class BasicTableColumns : ITuple, ITableColumns
	{
		IList<BasicColumnState> _ColumnStates;

		public int ColumnsCount { get { return _ColumnStates.Count; } }
		int ITuple.Length { get { return ColumnsCount; } }


		public BasicTableColumns(IList<IColumnInfo> columnInfos)
		{
			_ColumnStates = new List<BasicColumnState>(columnInfos.Count);
			for (int i = 0; i < columnInfos.Count; i++)
				_ColumnStates.Add(new BasicColumnState(columnInfos[i], false));
		}

		public BasicTableColumns(IList<BasicColumnInfo> columnInfos)
		{
			_ColumnStates = new List<BasicColumnState>(columnInfos.Count);
			for (int i = 0; i < columnInfos.Count; i++)
				_ColumnStates.Add(new BasicColumnState(columnInfos[i], false));
		}


		public IColumnState GetColumnState(int index)
		{
			return _ColumnStates[index];
		}

		public ITuple GetColumnsAsTuple()
		{
			return this;
		}

		/// <summary>Gets the title of a column</summary>
		object ITuple.GetValue(int index)
		{
			return _ColumnStates[index].Info.DisplayName;
		}

		/// <summary>Sets the title of a column</summary>
		void ITuple.SetValue(int index, object value)
		{
			_ColumnStates[index].Info.Name = value == null ? "" : value.ToString();
		}

		/// <summary>
		/// Sets the titles of all columns. <paramref name="newValues"/> should be of the same length of the current list, 
		/// i.e. only an existing list of columns can have its names modified
		/// </summary>
		void ITuple.ResetValues(IList newValues, bool cloneList)
		{
			if (_ColumnStates == null || newValues.Count != _ColumnStates.Count)
				throw new InvalidOperationException("Not supported for " + typeof(BasicTableColumns).Name + " if the count is different");

			for (int i = 0; i < newValues.Count; i++)
				(this as ITuple).SetValue(i, newValues[i]);
		}
	}
}