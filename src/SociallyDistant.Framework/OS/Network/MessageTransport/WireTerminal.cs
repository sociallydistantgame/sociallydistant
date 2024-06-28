#nullable enable

using System.Collections;
using System.Collections.Concurrent;

namespace SociallyDistant.Core.OS.Network.MessageTransport
{
	public class WireTerminal<T> :
			IDisposable,
			IReadOnlyCollection<T>
	{
		private ConcurrentQueue<T> outputQueue;
		private ConcurrentQueue<T> inputQueue;
		private Wire<T>? wire;

		public WireTerminal(ConcurrentQueue<T> input, ConcurrentQueue<T> output, Wire<T> wire)
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