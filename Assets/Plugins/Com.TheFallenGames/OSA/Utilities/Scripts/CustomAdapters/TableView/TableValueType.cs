using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	public enum TableValueType
	{
		/// <summary>
		/// Will try to call <see cref="String.ToString"/> on the value
		/// </summary>
		RAW,

		STRING,

		INT,

		LONG_INT,

		FLOAT,

		DOUBLE,

		/// <summary>
		/// Will try to cast the value to an integer, then retrieve its enum value using the <see cref="IColumnInfo.EnumValueType"/>. 
		/// If not successful, the raw integer will be shown
		/// </summary>
		ENUMERATION,

		BOOL,

		TEXTURE,
		// TBA
		//ARRAY
	}
}