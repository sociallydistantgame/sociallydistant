using System;
using System.Collections;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	public interface ITuple
	{
		int Length { get; }
		object GetValue(int index);
		void SetValue(int index, object value);
		void ResetValues(IList newValues, bool cloneList);
	}


	public static class ITupleExt
	{
		public static void CopyFrom(this ITuple tuple, IEnumerable list)
		{
			int i = 0;
			foreach (var item in list)
			{
				tuple.SetValue(i, item);
				++i;
			}
		}
	}
}