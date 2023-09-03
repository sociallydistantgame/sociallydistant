using System;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	public interface IColumnInfo
	{
		/// <summary>
		/// The column's simple name
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// The name to actually display, which can be different from <see cref="Name"/>
		/// </summary>
		string DisplayName { get; set; }

		TableValueType ValueType { get; }

		/// <summary>Only applicable to columns for which <see cref="ValueType"/> is <see cref="TableValueType.ENUMERATION"/>, in which case it becomes required/ </summary>
		Type EnumValueType { get; }

		/// <summary>The width, for vertical TableViews. -1 to use the prefab's size</summary>
		float Size { get; }
	}
}