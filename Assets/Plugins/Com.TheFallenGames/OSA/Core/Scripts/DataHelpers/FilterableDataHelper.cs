using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Com.TheFallenGames.OSA;
using System;
using UnityEngine.Events;
using frame8.Logic.Misc.Other.Extensions;
using frame8.Logic.Misc.Visual.UI;
using Com.TheFallenGames.OSA.Core;
using System.Collections.Generic;


namespace Com.TheFallenGames.OSA.DataHelpers
{  
	/// <summary>
	/// Data helper that sorts the given data and sorts it through a given predicate.
	/// 
	/// Set <see cref="UseFilteredData"/> to control whether you want to see the filtered or unfiltered sets.
	/// 
	/// Set <see cref="FilteringCriteria"/> to filter your dataset as needed, this does cause
	/// a rebuild of internal indexes at O(n) cost. 
	/// 
	/// Use <see cref="InsertItems(int, IList{T}, bool)"/> for bulk inserts.
	/// Make sure the index of what you insert is where you want it to be on the unfiltered set. 
	/// Make use of <see cref="GetUnfilteredIdex(int)"/> if you want to insert at a known filtered index.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FilterableDataHelper<T> : IEnumerable<T>
	{
        List<T> _FilteredList;
        List<T> _DataList;
        List<int> _FilteredItemIndex;
        bool _UseFilteredData = true;
        IOSA _Adapter;
        bool _KeepVelocityOnCountChange;
		Predicate<T> _FilteringCriteria;


		public FilterableDataHelper(IOSA iAdapter, bool keepVelocityOnCountChange = true) 
		{
			_DataList = new List<T>();
            _Adapter = iAdapter;
			_FilteredList = new List<T>();
			_FilteredItemIndex = new List<int>();
			_FilteringCriteria = (T) => true;
            _KeepVelocityOnCountChange = keepVelocityOnCountChange;
            
		}


		public bool UseFilteredData
		{
			get { return _UseFilteredData; }
			set { _UseFilteredData = value; }
		}

		public T this[int index]
		{
			get { return CurrentList[index]; }
		}

		public int Count { get { return CurrentList.Count; } }

		public Predicate<T> FilteringCriteria
		{
			set
			{
				if (value == null)
					throw new ArgumentNullException("Please make sure your predicate is valid");

				_FilteringCriteria = value;
				RemakeFilteredItems();

				_Adapter.ResetItems(CurrentList.Count);
			}
		}

		List<T> CurrentList
		{
			get { return _UseFilteredData ? _FilteredList : _DataList; }
		}

		#region IEnumerator<T> implementation
		IEnumerator<T> IEnumerable<T>.GetEnumerator() { return CurrentList.GetEnumerator(); }
		
		IEnumerator IEnumerable.GetEnumerator() { return CurrentList.GetEnumerator(); }
		#endregion


		#region ItemManipulation
		
		public void InsertItems(int index, IList<T> models, bool freezeEndEdge = false)
		{
			_DataList.InsertRange(index, models);
			int filteredIndex = _FilteredItemIndex.BinarySearch(index);
			bool inList = filteredIndex >= 0;
			if (!inList)
            {
				filteredIndex = (~filteredIndex); //this becomes 0 in empty lists
            }

			var modelsToAdd = new List<T>();
			var modelIndexes = new List<int>();

			for (int i = 0; i < models.Count; i++)
            {
				var currentModel = models[i];
                if (_FilteringCriteria(currentModel))
                {
					modelsToAdd.Add(currentModel);
					modelIndexes.Add(index + i);
                }
            }

			for(int i = filteredIndex; i < _FilteredItemIndex.Count; i++)
            {
				_FilteredItemIndex[i] += models.Count;
            }
			//The in list check is there mostly for cases where index is 0 the mathf max checks for bulk-add to empty lists

			_FilteredList.InsertRange(Mathf.Max(filteredIndex + (inList? 0 : -1), 0), modelsToAdd);
			_FilteredItemIndex.InsertRange(Mathf.Max(filteredIndex + (inList ? 0 : -1), 0), modelIndexes);


			if (_Adapter.InsertAtIndexSupported)
				if (_UseFilteredData)
					_Adapter.InsertItems(filteredIndex, modelsToAdd.Count, freezeEndEdge, _KeepVelocityOnCountChange);
				else
					_Adapter.InsertItems(index, models.Count, freezeEndEdge, _KeepVelocityOnCountChange);
			else
				_Adapter.ResetItems(CurrentList.Count, freezeEndEdge, _KeepVelocityOnCountChange);
		}
		public void InsertItemsAtStart(IList<T> models, bool freezeEndEdge = false) { InsertItems(0, models, freezeEndEdge); }
		public void InsertItemsAtEnd(IList<T> models, bool freezeEndEdge = false) { InsertItems(_DataList.Count, models, freezeEndEdge); }

		/// <summary>NOTE: Use <see cref="InsertItems(int, IList{T}, bool)"/> for bulk inserts, as it's way faster.
		/// Make sure the index is where you want it to be on the unfiltered set. 
		/// Make use of <see cref="GetUnfilteredIdex(int)"/> if you dont know the big dataset</summary>
		public void InsertOne(int unfilteredIndex, T model, bool freezeEndEdge = false)
		{
			_DataList.Insert(unfilteredIndex, model);

			int filteredIndex = _FilteredItemIndex.BinarySearch(unfilteredIndex);
			bool inList = filteredIndex >= 0;
			if (!inList)
			{
				filteredIndex = Mathf.Max((~filteredIndex)-1, 0); //this becomes 0 in empty lists
			}

            for (int i = filteredIndex; i < _FilteredItemIndex.Count; i++)
            {
				_FilteredItemIndex[i]++;
            }
			var isFilteredModel = _FilteringCriteria(model);

			if (isFilteredModel)
            {

				_FilteredList.Insert(filteredIndex, model);

				_FilteredItemIndex.Insert(filteredIndex, unfilteredIndex);
			}


			if (_Adapter.InsertAtIndexSupported)
			{
				var wetherToInsert = 1;
                if (UseFilteredData && !isFilteredModel)
                {
					wetherToInsert = 0;
                }
				_Adapter.InsertItems(_UseFilteredData ? filteredIndex : unfilteredIndex, wetherToInsert, freezeEndEdge, _KeepVelocityOnCountChange);
			}
			else
				_Adapter.ResetItems(CurrentList.Count, freezeEndEdge, _KeepVelocityOnCountChange);
		}
		public void InsertOneAtStart(T model, bool freezeEndEdge = false) { InsertOne(0, model, freezeEndEdge); }
		public void InsertOneAtEnd(T model, bool freezeEndEdge = false) { InsertOne(_DataList.Count, model, freezeEndEdge); }

		public void RemoveItems(int index, int count, bool freezeEndEdge = false)
		{

			_DataList.RemoveRange(index, count);



			int filteredMaxIndex = _FilteredItemIndex.BinarySearch(index+count);
			int filteredMinIndex = _FilteredItemIndex.BinarySearch(index);

			bool inListMin = filteredMinIndex >= 0;
			bool inListMax = filteredMaxIndex >= 0;
			if (!inListMin)
			{
				filteredMinIndex = (~filteredMinIndex); //this becomes 0 in empty lists
            }
            if (!inListMax)
            {
				filteredMaxIndex = (~filteredMaxIndex);
            }

			int filteredItemsToRemove = filteredMaxIndex - filteredMinIndex;


			for (int i = filteredMaxIndex; i < _FilteredItemIndex.Count; i++)
			{
				_FilteredItemIndex[i] -= count;
			}

			_FilteredItemIndex.RemoveRange(filteredMinIndex, filteredItemsToRemove);
			_FilteredList.RemoveRange(filteredMinIndex, filteredItemsToRemove);


			if (_Adapter.RemoveFromIndexSupported)
			{

				if (_UseFilteredData)
				{
					if(filteredItemsToRemove > 0)
						_Adapter.RemoveItems(filteredMinIndex, filteredItemsToRemove, freezeEndEdge, _KeepVelocityOnCountChange);

                }
				else 
				{
					_Adapter.RemoveItems(index, count, _KeepVelocityOnCountChange);
				}
			}
			else
				_Adapter.ResetItems(CurrentList.Count, freezeEndEdge, _KeepVelocityOnCountChange);
		}
		public void RemoveItemsFromStart(int count, bool freezeEndEdge = false) { RemoveItems(0, count, freezeEndEdge); }
		public void RemoveItemsFromEnd(int count, bool freezeEndEdge = false) { RemoveItems(_DataList.Count - count, count, freezeEndEdge); }

		/// <summary>NOTE: Use <see cref="RemoveItems(int, int, bool)"/> for bulk removes, as it's way faster</summary>
		public void RemoveOne(int unfilteredIndex, bool freezeEndEdge = false)
		{
			RemoveItems(unfilteredIndex, 1, freezeEndEdge);
		}
		public void RemoveOneFromStart(bool freezeEndEdge = false) { RemoveOne(0, freezeEndEdge); }
		public void RemoveOneFromEnd(bool freezeEndEdge = false) { RemoveOne(_DataList.Count - 1, freezeEndEdge); }

		/// <summary>
		/// NOTE: In case of resets, the preferred way is to clear the <see cref="List"/> yourself, add the models through it, and then call <see cref="NotifyListChangedExternally(bool)"/>.
		/// This saves memory by avoiding creating an intermediary array/list
		/// </summary>
		public void ResetItems(IList<T> models, bool freezeEndEdge = false)
		{
			_DataList.Clear();
			_DataList.AddRange(models);
			RemakeFilteredItems();
			_Adapter.ResetItems(CurrentList.Count, freezeEndEdge, _KeepVelocityOnCountChange);
		}

		/// <summary>
		/// In case of large lists of items, it is beneficial to replace the list instance, instead of using List's AddRange method, which <see cref="ResetItems(IList{T}, bool)"/> does.
		/// </summary>
		public void ResetItemsByReplacingListInstance(List<T> newListInstance, bool freezeEndEdge = false)
		{
			_DataList.Clear();
			_DataList = newListInstance;
			RemakeFilteredItems();
			_Adapter.ResetItems(CurrentList.Count, freezeEndEdge, _KeepVelocityOnCountChange);
		}

		public void NotifyListChangedExternally(bool freezeEndEdge = false)
		{
			RemakeFilteredItems();
			_Adapter.ResetItems(CurrentList.Count, freezeEndEdge, _KeepVelocityOnCountChange);
		}
		#endregion


		/// <summary>
		/// Call this function whenever you want to get the index of the filtered item on the unfiltered list
		/// </summary>
		/// <param name="filteredIndex"></param>
		/// <returns></returns>
		public int GetUnfilteredIdex(int filteredIndex)
        {
            if (!_UseFilteredData)
				return filteredIndex;

			
			return _FilteredItemIndex[filteredIndex];
			
		}

		void RemakeFilteredItems()
		{
			_FilteredList.Clear();
			_FilteredItemIndex.Clear();

			for (int i = 0; i < _DataList.Count; i++)
			{
				var model = _DataList[i];

				if (_FilteringCriteria(model))
				{
					_FilteredList.Add(model);
					_FilteredItemIndex.Add(i);
				}
			}
		}
	}
}