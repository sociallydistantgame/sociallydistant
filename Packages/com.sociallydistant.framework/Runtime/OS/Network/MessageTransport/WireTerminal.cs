#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

namespace OS.Network.MessageTransport
{
	public class WireTerminal<T> :
			IDisposable,
			IReadOnlyCollection<T>
	{
		private Queue<T> outputQueue;
		private Queue<T> inputQueue;
		private Wire<T>? wire;

		public WireTerminal(Queue<T> input, Queue<T> output, Wire<T> wire)
		{
			if (wire.TerminalA != null || wire.TerminalB != null)
				throw new InvalidOperationException("Cannot create a wire terminal for an existing wire! You are using this API incorrectly. Just create a new Wire<T> and it will create the corresponding WireTerminals for you.");
			
			this.inputQueue = input;
			this.outputQueue = output;
			this.wire = wire;
		}

		public bool IsDisposed => wire == null;
		
		public void Dispose()
		{
			Wire<T>? wireToDispose = wire;
			if (wireToDispose == null)
				return;

			this.wire = null;
			wireToDispose.Dispose();
		}

		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator()
		{
			ThrowIfDisposed();
			return inputQueue.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public int Count
		{
			get
			{
				ThrowIfDisposed();
				return this.inputQueue.Count;
			}
		}

		public void Enqueue(T value)
		{
			ThrowIfDisposed();
			outputQueue.Enqueue(value);
		}

		public T Dequeue()
		{
			ThrowIfDisposed();
			return inputQueue.Dequeue();
		}

		public bool TryDequeue(out T value)
		{
			ThrowIfDisposed();
			return inputQueue.TryDequeue(out value);
		}

		private void ThrowIfDisposed()
		{
			if (wire == null)
				throw new ObjectDisposedException(this.GetType().Name);
		}
	}
}