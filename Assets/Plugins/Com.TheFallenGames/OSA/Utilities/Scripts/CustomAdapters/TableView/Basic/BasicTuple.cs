using System.Collections;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Basic
{
	public class BasicTuple : ITuple
	{
		public int Length { get { return _Values.Count; } }

		IList _Values;


		/// <summary>See <see cref="ResetValues(IList, bool)"/></summary>
		public BasicTuple() { }

		/// <summary>See <see cref="ResetValues(IList, bool)"/></summary>
		public BasicTuple(IList values, bool cloneList = false)
		{
			ResetValues(values, cloneList);
		}


		public object GetValue(int index)
		{
			return _Values[index];
		}

		public void SetValue(int index, object value)
		{
			_Values[index] = value;
		}

		/// <summary>
		/// Passing <paramref name="cloneList"/>=true, will clone the list of values. Otherwise, will keep a reference to the list 
		/// and thus will be affected by external changes to it
		/// </summary>
		public void ResetValues(IList newValues, bool cloneList)
		{
			if (cloneList)
			{
				_Values = new object[newValues.Count];
				for (int i = 0; i < newValues.Count; i++)
					_Values[i] = newValues[i];
			}
			else
				_Values = newValues;
		}
	}
}