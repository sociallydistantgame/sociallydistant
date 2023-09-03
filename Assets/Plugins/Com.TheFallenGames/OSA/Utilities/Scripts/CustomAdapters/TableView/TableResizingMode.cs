namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	public enum TableResizingMode
	{
		NONE,

		AUTO_FIT_TUPLE_CONTENT,

		/// <summary>Not implemented yet</summary>
		MANUAL_COLUMNS,

		MANUAL_TUPLES,

		/// <summary>Not fully implemented yet. Behaves exactly like <see cref="MANUAL_TUPLES"/></summary>
		MANUAL_COLUMNS_AND_TUPLES,
	}
}