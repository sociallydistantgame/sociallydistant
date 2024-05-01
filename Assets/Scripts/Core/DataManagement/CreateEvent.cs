using System;
using System.Collections.Generic;

namespace Core.DataManagement
{
	public class CreateEvent
	{
		private readonly ThreadInvoker threadInvoker;
		private readonly DataEventDispatcher dispatcher;
		private readonly Queue<Action> pendingObjects = new();
		private Dictionary<Type, List<object>> invocationLists = new Dictionary<Type, List<object>>();

		public CreateEvent(DataEventDispatcher dispatcher, ThreadInvoker threadInvoker)
		{
			this.threadInvoker = threadInvoker;
			this.dispatcher = dispatcher;
		}
		
		public void AddCallback<TDataElement>(CreateCallback<TDataElement> callback)
			where TDataElement : struct
		{
			Type type = typeof(TDataElement);

			if (!invocationLists.TryGetValue(type, out List<object> invocationList))
			{
				invocationList = new List<object>();
				invocationLists.Add(type, invocationList);
			}

			invocationList.Add(callback);
		}
		
		public void RemoveCallback<TDataElement>(CreateCallback<TDataElement> callback)
			where TDataElement : struct
		{
			Type type = typeof(TDataElement);

			if (!invocationLists.TryGetValue(type, out List<object> invocationList))
				return;

			invocationList.Remove(callback);

			if (invocationList.Count == 0)
				invocationLists.Remove(type);
		}

		public void InvokePending()
		{
			if (dispatcher.PauseEvents)
				return;
			
			while (pendingObjects.Count > 0)
			{
				Action action = pendingObjects.Dequeue();
				action?.Invoke();
			}
		}
		
		public void Invoke<TDataElement>(TDataElement data)
			where TDataElement : struct
		{
			if (dispatcher.PauseEvents)
			{
				// Defer the invocation until later.
				pendingObjects.Enqueue(() => { Invoke<TDataElement>(data); });
				return;
			}

			Type type = typeof(TDataElement);
			
			if (!invocationLists.TryGetValue(type, out List<object> invocationList))
				return;

			foreach (object invocation in invocationList)
			{
				if (invocation is CreateCallback<TDataElement> callback)
				{
					threadInvoker.Do(() =>
					{
						callback(data);
					});
				}
			}
		}
	}
}