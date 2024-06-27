using GamePlatform;

namespace Core.DataManagement
{
	public class DataEventDispatcher
	{
		private readonly CreateEvent createEvent;
		private readonly DeleteEvent deleteEvent;
		private readonly ModifyEvent modifyEvent;

		private bool isPaused;
		
		public bool PauseEvents
		{
			get => isPaused;
			set
			{
				if (isPaused == value)
					return;

				isPaused = value;

				if (!isPaused)
					createEvent.InvokePending();
			}
		}
		
		public CreateEvent Create => createEvent;
		public DeleteEvent Delete => deleteEvent;
		public ModifyEvent Modify => modifyEvent;

		public DataEventDispatcher(GameManager gameManager)
		{
			var threadInvoker = new ThreadInvoker(gameManager);
			createEvent = new CreateEvent(this, threadInvoker);
			deleteEvent = new DeleteEvent(threadInvoker);
			modifyEvent = new ModifyEvent(threadInvoker);
		}
	}
}