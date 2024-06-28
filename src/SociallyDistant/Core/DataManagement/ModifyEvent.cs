using SociallyDistant.Core.Core;

namespace SociallyDistant.Core.DataManagement
{
	public class ModifyEvent
	{
		private Dictionary<Type, List<object>> invocationLists = new Dictionary<Type, List<object>>();
		private readonly ThreadInvoker threadInvoker;

		public ModifyEvent(ThreadInvoker threadInvoker)
		{
			this.threadInvoker = threadInvoker;
		}
		
		public void AddCallback<TDataElement>(ModifyCallback<TDataElement> callback)
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
		
		public void RemoveCallback<TDataElement>(ModifyCallback<TDataElement> callback)
			where TDataElement : struct
		{
			Type type = typeof(TDataElement);

			if (!invocationLists.TryGetValue(type, out List<object> invocationList))
				return;

			invocationList.Remove(callback);

			if (invocationList.Count == 0)
				invocationLists.Remove(type);
		}

		public void Invoke<TDataElement>(TDataElement subjectPrevious, TDataElement subjectNew)
			where TDataElement : struct
		{
			Type type = typeof(TDataElement);
			
			if (!invocationLists.TryGetValue(type, out List<object> invocationList))
				return;

			foreach (object invocation in invocationList)
			{
				if (invocation is ModifyCallback<TDataElement> callback)
				{
					threadInvoker.Do(() =>
					{
						callback(subjectPrevious, subjectNew);
					});
				}
			}
		}
	}
}