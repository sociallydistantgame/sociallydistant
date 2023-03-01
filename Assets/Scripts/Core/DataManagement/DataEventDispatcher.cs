namespace Core.DataManagement
{
	public class DataEventDispatcher
	{
		private readonly CreateEvent createEvent = new CreateEvent();
		private readonly DeleteEvent deleteEvent = new DeleteEvent();
		private readonly ModifyEvent modifyEvent = new ModifyEvent();

		public CreateEvent Create => createEvent;
		public DeleteEvent Delete => deleteEvent;
		public ModifyEvent Modify => modifyEvent;
	}
}