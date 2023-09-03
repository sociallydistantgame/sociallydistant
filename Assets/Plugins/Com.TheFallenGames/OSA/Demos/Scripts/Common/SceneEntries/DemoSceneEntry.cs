using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.Demos.Common.Drawer;

namespace Com.TheFallenGames.OSA.Demos.Common.SceneEntries
{
	/// <summary>
	/// <para>This is used in demos and not needed in production. Its main purpose is to make it easier to execute a demo scene's initialization code when there's more than 1 adapter</para> 
	/// <para>You'll normally use your implementation of OSA just as any other separate component in your application, adding/removing items using the provided public methods.</para> 
	/// <para>Just make sure to initialize OSA before using it, if it didn't already initialize itself. Please read the documentation for <see cref="OSA{TParams, TItemViewsHolder}.Init"/></para> 
	/// </summary>
	public abstract class DemoSceneEntry<TAdapter, TParams, TViewsHolder> : MonoBehaviour 
		where TAdapter : OSA<TParams, TViewsHolder>
		where TParams : BaseParams
		where TViewsHolder : BaseItemViewsHolder
	{
		protected TAdapter[] _Adapters;
		protected DrawerCommandPanel _Drawer;

		int _InitializedAdapters;


		protected virtual void Awake()
		{

		}

		protected virtual void Start()
		{
			InitAdapters();

			_Drawer = GameObject.Find(typeof(DrawerCommandPanel).Name).GetComponent<DrawerCommandPanel>();
			InitDrawer();

			_InitializedAdapters = 0;
			foreach (var adapter in _Adapters)
			{
				var copyOfAdapter = adapter;
				if (copyOfAdapter.IsInitialized)
					++_InitializedAdapters;
				else
				{
					Action onInitializedWithAutoUnsubscribe = null;
					onInitializedWithAutoUnsubscribe = () =>
					{
						copyOfAdapter.Initialized -= onInitializedWithAutoUnsubscribe;
						OnAdapterInitialized();
					};
					copyOfAdapter.Initialized += onInitializedWithAutoUnsubscribe;
				}
			}

			// If not all initialized, OnAllAdaptersInitialized will be called when OnAdapterInitialized will be called for last one 
			if (_InitializedAdapters == _Adapters.Length)
				OnAllAdaptersInitialized();
		}

		protected virtual void Update() { }

		protected virtual void InitAdapters()
		{
			_Adapters = FindObjectsOfType<TAdapter>();
		}

		protected abstract void InitDrawer();

		protected virtual void OnAdapterInitialized()
		{
			if (++_InitializedAdapters == _Adapters.Length)
				OnAllAdaptersInitialized();
		}

		protected virtual void OnAllAdaptersInitialized()
		{
			_Drawer.AddItemRequested += OnAddItemRequested;
			_Drawer.RemoveItemRequested += OnRemoveItemRequested;
			_Drawer.ItemCountChangeRequested += OnItemCountChangeRequested;
		}

		#region events from DrawerCommandPanel
		void OnAddItemRequested(int index)
		{
			foreach (var adapter in _Adapters)
				OnAddItemRequested(adapter, index);
		}

		void OnRemoveItemRequested(int index)
		{
			foreach (var adapter in _Adapters)
				OnRemoveItemRequested(adapter, index);
		}

		void OnItemCountChangeRequested(int count)
		{
			foreach (var adapter in _Adapters)
				OnItemCountChangeRequested(adapter, count);
		}

		protected virtual void OnAddItemRequested(TAdapter adapter, int index)
		{

		}

		protected virtual void OnRemoveItemRequested(TAdapter adapter, int index)
		{

		}

		protected virtual void OnItemCountChangeRequested(TAdapter adapter, int count)
		{

		}
		#endregion
	}
}
