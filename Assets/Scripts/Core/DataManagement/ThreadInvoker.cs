using System;
using System.Threading;
using GamePlatform;

namespace Core.DataManagement
{
	public sealed class ThreadInvoker
	{
		private readonly GameManager game;
		private readonly int desiredThreadId;

		public ThreadInvoker(GameManager game)
		{
			this.game = game;
			this.desiredThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public void Do(Action action)
		{
			int currentTHread = Thread.CurrentThread.ManagedThreadId;
			if (currentTHread != desiredThreadId)
			{
				GameManager.ScheduleAction(action);
				return;
			}

			action();
		}
	}
}