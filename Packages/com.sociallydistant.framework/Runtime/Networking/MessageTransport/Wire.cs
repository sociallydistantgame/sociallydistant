#nullable enable
using System;
using System.Collections.Generic;

namespace Networking.MessageTransport
{
	public class Wire<T>
		: IDisposable
	{
		private readonly Queue<T> laneA;
		private readonly Queue<T> laneB;
		private WireTerminal<T> terminalA;
		private WireTerminal<T> terminalB;

		public WireTerminal<T> TerminalA => terminalA;
		public WireTerminal<T> TerminalB => terminalB;

		public Wire()
		{
			laneA = new Queue<T>();
			laneB = new Queue<T>();

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