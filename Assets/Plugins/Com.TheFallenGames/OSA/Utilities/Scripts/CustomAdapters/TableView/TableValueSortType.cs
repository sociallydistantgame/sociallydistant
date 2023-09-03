using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	public enum TableValueSortType
	{
		/// <summary>
		/// The default order in the provided data. Once changed, it'll only have one of <see cref="ASCENDING"/> or <see cref="DESCENDING"/>
		/// </summary>
		NONE,

		ASCENDING,
		DESCENDING
	}
}