using System;
using System.Collections.Generic;

namespace Core.DataManagement
{
	public class ModifyEvent
	{
		public delegate void ModifyCallback<TDataElement>(TDataElement subjectPrevious, TDataElement subjectNew)
			where TDataElement : struct, IDataWithId;

		private Dictionary<Type, List<object>> invocationLists = new Dictionary<Type, List<object>>();

		public void AddCallback<TDataElement>(ModifyCallback<TDataElement> callback)
			where TDataElement : struct, IDataWithId
		{
			Type type = typeof(TDataElement);

			if (!invocationLists.TryGetValue(type, out List<object> invocationList))
			{
				invocationList = new List<object>();
				invocationLists.Add(type, invocationList);
			}

			invocationList.Add(callback);
		}
		
		public void RemoveCallback<TDataElement>(ModifyCallback<TDataElement> callback)
			where TDataElement : struct, IDataWithId
		{
			Type type = typeof(TDataElement);

			if (!invocationLists.TryGetValue(type, out List<object> invocationList))
				return;

			invocationList.Remove(callback);

			if (invocationList.Count == 0)
				invocationLists.Remove(type);
		}

		public void Invoke<TDataElement>(TDataElement subjectPrevious, TDataElement subjectNew)
			where TDataElement : struct, IDataWithId
		{
			Type type = typeof(TDataElement);
			
			if (!invocationLists.TryGetValue(type, out List<object> invocationList))
				return;

			foreach (object invocation in invocationList)
			{
				if (invocation is ModifyCallback<TDataElement> callback)
					callback(subjectPrevious, subjectNew);
			}
		}
	}
}