

using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	public static class TableViewUtil
	{
		public static TTuple CreateTupleWithEmptyValues<TTuple>(int length)
			where TTuple : ITuple, new()
		{
			var emptyValues = new List<object>(length);
			for (int i = 0; i < length; i++)
				emptyValues.Add(null);

			var t = new TTuple();
			t.ResetValues(emptyValues, false);

			return t;
		}
	}
}
