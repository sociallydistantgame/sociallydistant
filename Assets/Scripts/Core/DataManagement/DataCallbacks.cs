namespace Core.DataManagement
{
	public class DataCallbacks : IWorldDataCallbacks
	{
		private readonly DataEventDispatcher eventDispatcher;

		public DataCallbacks(DataEventDispatcher eventDispatcher)
		{
			this.eventDispatcher = eventDispatcher;
		}

		public void AddCreateCallback<TDataElement>(CreateCallback<TDataElement> callback)
			where TDataElement : struct
		{
			eventDispatcher.Create.AddCallback(callback);
		}
		
		public void RemoveCreateCallback<TDataElement>(CreateCallback<TDataElement> callback)
			where TDataElement : struct
		{
			eventDispatcher.Create.RemoveCallback(callback);
		}
		
		public void AddDeleteCallback<TDataElement>(DeleteCallback<TDataElement> callback)
			where TDataElement : struct
		{
			eventDispatcher.Delete.AddCallback(callback);
		}
		
		public void RemoveDeleteCallback<TDataElement>(DeleteCallback<TDataElement> callback)
			where TDataElement : struct
		{
			eventDispatcher.Delete.RemoveCallback(callback);
		}
		
		public void AddModifyCallback<TDataElement>(ModifyCallback<TDataElement> callback)
			where TDataElement : struct
		{
			eventDispatcher.Modify.AddCallback(callback);
		}
		
		public void RemoveModifyCallback<TDataElement>(ModifyCallback<TDataElement> callback)
			where TDataElement : struct
		{
			eventDispatcher.Modify.RemoveCallback(callback);
		}
	}
}