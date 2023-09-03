using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using frame8.Logic.Misc.Other.Extensions;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple
{
	public interface ITupleAdapter : IOSA
	{
		event Action<TupleValueViewsHolder> ValueClicked;
		event Action<TupleValueViewsHolder, object> ValueChangedFromInput;

		TupleParams TupleParameters { get; }
		ITupleAdapterSizeHandler SizeHandler { get; set; }

		void ResetWithTuple(ITuple tupleModel, ITableColumns columnsProvider);
		void ForceUpdateValueViewsHolderIfVisible(int withItemIndex);
		void ForceUpdateValueViewsHolder(TupleValueViewsHolder vh);
		void OnWillBeRecycled(float newSize);
	}
}
