using System;
using System.Collections.Generic;

namespace Core.DataManagement
{
	public class CreateEvent
	{
		public delegate void CreateCallback<TDataElement>(TDataElement subject)
			where TDataElement : struct;

		private Dictionary<Type, List<object>> invocationLists = new Dictionary<Type, List<object>>();

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

		public void Invoke<TDataElement>(TDataElement data)
			where TDataElement : struct
		{
			Type type = typeof(TDataElement);
			
			if (!invocationLists.TryGetValue(type, out List<object> invocationList))
				return;

			foreach (object invocation in invocationList)
			{
				if (invocation is CreateCallback<TDataElement> callback)
					callback(data);
			}
		}
	}
}