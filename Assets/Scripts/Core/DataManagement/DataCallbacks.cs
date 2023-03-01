namespace Core.DataManagement
{
	public class DataCallbacks
	{
		private readonly DataEventDispatcher eventDispatcher;

		public DataCallbacks(DataEventDispatcher eventDispatcher)
		{
			this.eventDispatcher = eventDispatcher;
		}

		public void AddCreateCallback<TDataElement>(CreateEvent.CreateCallback<TDataElement> callback)
			where TDataElement : struct, IDataWithId
		{
			eventDispatcher.Create.AddCallback(callback);
		}
		
		public void RemoveCreateCallback<TDataElement>(CreateEvent.CreateCallback<TDataElement> callback)
			where TDataElement : struct, IDataWithId
		{
			eventDispatcher.Create.RemoveCallback(callback);
		}
		
		public void AddDeleteCallback<TDataElement>(DeleteEvent.DeleteCallback<TDataElement> callback)
			where TDataElement : struct, IDataWithId
		{
			eventDispatcher.Delete.AddCallback(callback);
		}
		
		public void RemoveDeleteCallback<TDataElement>(DeleteEvent.DeleteCallback<TDataElement> callback)
			where TDataElement : struct, IDataWithId
		{
			eventDispatcher.Delete.RemoveCallback(callback);
		}
		
		public void AddModifyCallback<TDataElement>(ModifyEvent.ModifyCallback<TDataElement> callback)
			where TDataElement : struct, IDataWithId
		{
			eventDispatcher.Modify.AddCallback(callback);
		}
		
		public void RemoveModifyCallback<TDataElement>(ModifyEvent.ModifyCallback<TDataElement> callback)
			where TDataElement : struct, IDataWithId
		{
			eventDispatcher.Modify.RemoveCallback(callback);
		}
	}
}