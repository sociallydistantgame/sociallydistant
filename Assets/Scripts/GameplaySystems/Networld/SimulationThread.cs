#nullable enable
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace GameplaySystems.Networld
{
	public sealed class SimulationThread
	{
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
			
			Debug.Log("Pausing the simulation...");
			continueUpdates.Reset();

			Debug.Log("Waiting for current simulation update to complete...");
			currentUpdate.WaitOne();

			Debug.Log("Simulation thread is now asleep. Internet graph is safe to update.");
		}

		public void StartSimulation()
		{
			if (simulationRunning)
				return;

			Debug.Log("Waking the internet simulation thread up. It will no longer be safe to modify the internet graph.");
			
			simulationThread = new Thread(RunSimulation);
			simulationThreadId = simulationThread.ManagedThreadId;
			
			simulationRunning = true;
			threadFinished.Reset();
			continueUpdates.Set();

			Debug.Log("Simulation sync state reset, ready to go vroom vroom.");
			
			simulationThread.Start();

			Debug.Log("Internet is now up!");
		}

		public void StopSimulation()
		{
			if (!simulationRunning)
				return;

			Debug.Log("Starting simulation shutdown...");
			
			this.continueUpdates.Reset();
			this.simulationRunning = false;
			this.continueUpdates.Set();
			
			Debug.Log("Waiting for the last simulation update to complete, main thread will hang for a bit.");
			
			threadFinished.WaitOne();
			
			Debug.Log("The Internet is now down!");
		}
		
		private async void RunSimulation()
		{
			var stopwatch = new Stopwatch();
            
			while (simulationRunning)
			{
				stopwatch.Start();
				currentUpdate.Set();
				await coreRouter.NetworkUpdate();
				currentUpdate.Reset();
				stopwatch.Stop();

				if (stopwatch.Elapsed.TotalSeconds > 1)
				{
					Debug.LogError($"Simulation update took more than 1 second (total time: {stopwatch.Elapsed}). Consider sending less packets.");
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
	}
}