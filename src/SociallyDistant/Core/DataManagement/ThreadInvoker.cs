namespace SociallyDistant.Core.DataManagement
{
	public sealed class ThreadInvoker
	{
		private readonly SociallyDistantGame game;
		private readonly int desiredThreadId;

		internal ThreadInvoker(SociallyDistantGame game)
		{
			this.game = game;
			this.desiredThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public void Do(Action action)
		{
			int currentTHread = Thread.CurrentThread.ManagedThreadId;
			if (currentTHread != desiredThreadId)
			{
				SociallyDistantGame.ScheduleAction(action);
				return;
			}

			action();
		}
	}
}