using System;
using UnityEngine;
using Com.TheFallenGames.OSA.DataHelpers;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Extra
{
	/// <summary>
	/// Class for loading data via a <see cref="AsyncBufferredTableData{TTuple}"/> and notifying a <see cref="TableAdapter{TParams, TTupleViewsHolder, THeaderTupleViewsHolder}"/>
	/// about changes, while staying aware of its lifecycle events like <see cref="TableAdapter{TParams, TTupleViewsHolder, THeaderTupleViewsHolder}.ChangeItemsCount(Core.ItemCountChangeMode, int, int, bool, bool)"/>
	/// and invalidating any existing loading tasks when needed.
	/// It'll dispose everything on first count change.
	/// </summary>
	public class AsyncLoadingUIController<TTuple>
		where TTuple : ITuple, new()
	{
		ITableAdapter _Adapter;
		AsyncBufferredTableData<TTuple> _AsyncData;
		static int _NumInstances; // just for debug purposes

		int _ID = _NumInstances++;


		public AsyncLoadingUIController(ITableAdapter tableAdapter, AsyncBufferredTableData<TTuple> data)
		{
			_Adapter = tableAdapter;
			_AsyncData = data;
			_AsyncData.Source.LoadingSessionStarted += OnAsyncDataLoadingStarted;
			_AsyncData.Source.SingleTaskFinished += OnAsyncDataSingleTaskFinished;
			_AsyncData.Source.LoadingSessionEnded += OnAsyncDataLoadingSessionEnded;
		}

		public void BeginListeningForSelfDisposal()
		{
			_Adapter.ItemsRefreshed += OnAdapterItemsRefreshed;
		}

		void OnAsyncDataLoadingStarted()
		{
			if (_Adapter.Options != null)
				_Adapter.Options.IsLoading = true;
		}

		void OnAsyncDataSingleTaskFinished(AsyncBufferredDataSource<TTuple>.LoadingTask task)
		{
			// Make sure the adapter wasn't disposed meanwhile
			if (_Adapter == null || !_Adapter.IsInitialized)
			{
				Dispose();
				return;
			}

			_Adapter.RefreshRange(task.FirstItemIndex, task.CountToRead);
		}

		void OnAsyncDataLoadingSessionEnded()
		{
			// Make sure the adapter wasn't disposed meanwhile
			if (_Adapter == null || !_Adapter.IsInitialized)
			{
				Dispose();
				return;
			}

			if (_Adapter.Options != null)
				_Adapter.Options.IsLoading = false;
		}

		// If the adapter refreshes its items while one or more tasks are in progress, 
		// make sure to invalidate them so they'll be ignored when they'll fire OnFinishedOneTask
		void OnAdapterItemsRefreshed(int prevCount, int newCount)
		{
			// When the adapter's items count changes or the views are fully refreshed (for example, as a result 
			// of resizing the ScrollView), it fires the ItemsRefreshed.
			// Check whether that was a result of a simple Refresh (which is done by 
			// OSA and thus preserves the data) or an external ResetTable call, which changes the data and thus 
			// requires this uiController to self-dispose. 
			bool sameDataReferences = _Adapter.Tuples == _AsyncData && _Adapter.Columns == _AsyncData.Columns;

			if (sameDataReferences)
				return;

			int numRunning = _AsyncData.Source.CurrentlyLoadingTasksCount;
			if (numRunning > 0)
			{
				//if (_AsyncData.ShowLogs)
				//	Debug.Log("OnAdapterItemsRefreshed(count " + prevCount + " -> " + newCount + ") : Clearing all " + numRunning + " active tasks");

				// Update: this overrides an existing loading task, so it's left to the adapter's will to disable the loading state
				//if (_Adapter.Options != null)
				//	_Adapter.Options.IsLoading = false;

				_AsyncData.Source.ClearAllRunningTasks();
			}

			if (_AsyncData.Source.ShowLogs)
				Debug.Log(
					"AsyncLoadingUIController #"+ _ID + 
					": OnAdapterItemsRefreshed(count " + prevCount + " -> " + newCount + 
					") : Clearing all " + numRunning + " active tasks and disposing self"
				);

			Dispose();
		}

		void Dispose()
		{
			// Unsubscribing from events makes this object available for GC

			if (_AsyncData != null && _AsyncData.Source != null)
			{
				_AsyncData.Source.LoadingSessionStarted -= OnAsyncDataLoadingStarted;
				_AsyncData.Source.SingleTaskFinished -= OnAsyncDataSingleTaskFinished;
				_AsyncData.Source.LoadingSessionEnded -= OnAsyncDataLoadingSessionEnded;
			}
			if (_Adapter != null)
				_Adapter.ItemsRefreshed -= OnAdapterItemsRefreshed;

			_Adapter = null;
			_AsyncData = null;
		}
	}
}