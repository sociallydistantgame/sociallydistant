#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OS.Network.MessageTransport
{
	public class Wire<T>
		: IDisposable
	{
		private readonly ConcurrentQueue<T> laneA;
		private readonly ConcurrentQueue<T> laneB;
		private WireTerminal<T> terminalA;
		private WireTerminal<T> terminalB;
		
		public WireTerminal<T> TerminalA => terminalA;
		public WireTerminal<T> TerminalB => terminalB;

		public Wire()
		{
			laneA = new ConcurrentQueue<T>();
			laneB = new ConcurrentQueue<T>();

			var termA = new WireTerminal<T>(laneA, laneB, this);
			var termB = new WireTerminal<T>(laneB, laneA, this);

			this.terminalA = termA;
			this.terminalB = termB;
		}

		public bool IsDisposed { get; private set; }
		
		/// <inheritdoc />
		public void Dispose()
		{
			IsDisposed = true;
			laneA.Clear();
			laneB.Clear();
			terminalA.Dispose();
			terminalB.Dispose();
		}
	}
}