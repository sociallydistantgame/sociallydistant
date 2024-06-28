#nullable enable
using System.Collections.Concurrent;

namespace SociallyDistant.Core.OS.Network.MessageTransport
{
	/// <summary>
	///		Represents a queue of work items to be completed by a thread.
	/// </summary>
	/// <remarks>
	///		This class is designed for multi-threaded workloads. A programmer can create a work queue
	///		and give it to a thread that's meant to perform all work in the queue. Other threads can then
	///		enqueue work to be completed by the original thread's work queue.
	///
	///		Work is completed by the work queue in the order that it is enqueued, with the most-recent work item being
	///		completed last.
	///
	///		A work queue can also be used to allow background threads to execute work on the main thread, for example to
	///		access thread-unsafe Unity APIs.
	/// </remarks>
	public class WorkQueue
	{
		private readonly ConcurrentQueue<PendingWork> workToDo = new ConcurrentQueue<PendingWork>();
		
		private int maximumWork = 256;

		/// <summary>
		///		Gets or sets how many work items in the queue can be executed by the work thread at once.
		///		If set to 0, there is no maximum amount of work and the thread will execute work items until the queue
		///		is empty.
		/// </summary>
		public int MaximumWorkPerUpdate
		{
			get => maximumWork;
			set => maximumWork = value;
		}
		
		/// <summary>
		///		Executes any pending work in the queue.
		/// </summary>
		public void RunPendingWork()
		{
			var workDone = 0;
			while (workToDo.TryDequeue(out PendingWork work))
			{
				work.DoWork();

				workDone++;
				
				if (maximumWork > 0 && workDone == maximumWork)
					break;
			}
		}

		/// <summary>
		///		Adds a new work item to the queue.
		/// </summary>
		/// <param name="work">An action representing the work to be completed.</param>
		public void Enqueue(Action work)
		{
			this.workToDo.Enqueue(new PendingWork(work));
		}

		/// <summary>
		///		Adds a new work item to the queue and returns an awaitable task that completes when the work is completed.
		/// </summary>
		/// <param name="work">An action representing the work to be completed</param>
		/// <returns>A task that completes when the work item is executed.</returns>
		public Task EnqueueAsync(Action work)
		{
			var pendingWork = new PendingWork(work);

			this.workToDo.Enqueue(pendingWork);

			return pendingWork.Task;
		}
		
		
		
		private class PendingWork
		{
			private readonly TaskCompletionSource<Action> completionSource;
			private readonly Action work;

			public Task Task => completionSource.Task;
			
			public PendingWork(Action work)
			{
				this.work = work;
				this.completionSource = new TaskCompletionSource<Action>();
			}

			public void DoWork()
			{
				try
				{
					work();
					completionSource.SetResult(work);
				}
				catch (Exception ex)
				{
					completionSource.SetException(ex);
					throw;
				}
			}
		}
	}
}