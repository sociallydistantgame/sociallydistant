#nullable enable
using System.Diagnostics;
using Serilog;
using SociallyDistant.Core.OS.Network.MessageTransport;

namespace SociallyDistant.GameplaySystems.Networld
{
	public sealed class SimulationThread
	{
		private static readonly WorkQueue workQueue = new();
		
		private static SimulationThread? instance;
		
		private readonly CoreRouter coreRouter;
		private readonly ManualResetEvent threadFinished = new ManualResetEvent(false);
		private readonly ManualResetEvent currentUpdate = new ManualResetEvent(true);
		private readonly ManualResetEvent continueUpdates = new ManualResetEvent(true);
		private Thread? simulationThread;
		private int simulationThreadId;

		private volatile bool simulationRunning = false;
		
		public SimulationThread(CoreRouter coreRouter)
		{
			instance = this;
			
			this.coreRouter = coreRouter;
		}

		public void PauseSimulationUpdates()
		{
			if (!simulationRunning)
				return;
			
			if (Thread.CurrentThread.ManagedThreadId == simulationThreadId)
				throw new InvalidOperationException("Cannot pause simulation updates on the simulation thread, since this will dead-lock the game.");
			
			Log.Information("Pausing the simulation...");
			continueUpdates.Reset();

			Log.Information("Waiting for current simulation update to complete...");
			currentUpdate.WaitOne();

			Log.Information("Simulation thread is now asleep. Internet graph is safe to update.");
		}

		public void StartSimulation()
		{
			if (simulationRunning)
				return;

			Log.Information("Waking the internet simulation thread up. It will no longer be safe to modify the internet graph.");
			
			simulationThread = new Thread(RunSimulation);
			simulationThreadId = simulationThread.ManagedThreadId;
			
			simulationRunning = true;
			threadFinished.Reset();
			continueUpdates.Set();

			Log.Information("Simulation sync state reset, ready to go vroom vroom.");
			
			simulationThread.Start();

			Log.Information("Internet is now up!");
		}

		public void StopSimulation()
		{
			if (!simulationRunning)
				return;

			Log.Information("Starting simulation shutdown...");
			
			this.continueUpdates.Reset();
			this.simulationRunning = false;
			this.continueUpdates.Set();
			
			Log.Information("Waiting for the last simulation update to complete, main thread will hang for a bit.");
			
			threadFinished.WaitOne();
			
			Log.Information("The Internet is now down!");
		}
		
		private void RunSimulation()
		{
			var stopwatch = new Stopwatch();
            
			while (simulationRunning)
			{
				stopwatch.Start();
				
				currentUpdate.Set();

				try
				{
					workQueue.RunPendingWork();
					coreRouter.NetworkUpdate();
				}
				catch (Exception ex)
				{
					Log.Error("Network simulation went ape-shit and threw an exception.");
					Log.Error(ex.ToString());
				}

				currentUpdate.Reset();
				stopwatch.Stop();

				if (stopwatch.Elapsed.TotalSeconds > 1)
				{
					Log.Warning($"Simulation update took more than 1 second (total time: {stopwatch.Elapsed}). Consider sending less packets.");
				}

				stopwatch.Reset();
				continueUpdates.WaitOne();
			}

			threadFinished.Set();
		}

		public static void Synchronize()
		{
			if (instance == null)
				return;

			if (!instance.simulationRunning)
				return;

			instance.currentUpdate.WaitOne();
		}
		
		public static async Task SynchronizeAsync()
		{
			await Task.Run(Synchronize);
		}

		public static void ScheduleWork(Action action)
		{
			workQueue.Enqueue(action);
		}
	}
}