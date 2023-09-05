using System.Linq;
using System.Text;
using Networking.MessageTransport;
using NUnit.Framework;

namespace Tests.EditMode_Tests
{
	public class MessageTransportTests
	{
		[Test]
		public void WireTerminalADisposesTerminalB()
		{
			var wire = new Wire<byte>();

			wire.TerminalA.Dispose();
			Assert.IsTrue(wire.TerminalB.IsDisposed);
		}

		[Test]
		public void WireTerminalBDisposesTerminalA()
		{
			var wire = new Wire<byte>();

			wire.TerminalB.Dispose();
			Assert.IsTrue(wire.TerminalA.IsDisposed);
		}

		[Test]
		public void WireDisposesBothTerminals()
		{
			var wire = new Wire<byte>();
			wire.Dispose();
			
			Assert.IsTrue(wire.TerminalA.IsDisposed);
			Assert.IsTrue(wire.TerminalB.IsDisposed);
		}
		
		[Test]
		public void WireTerminalDisposesEntireWire()
		{
			var wire = new Wire<byte>();

			wire.TerminalA.Dispose();

			Assert.IsTrue(wire.TerminalA.IsDisposed);
			Assert.IsTrue(wire.TerminalB.IsDisposed);
			
			Assert.IsTrue(wire.IsDisposed);
		}

		[Test]
		public void WireTerminalsAreDistinct()
		{
			using var wire = new Wire<byte>();

			Assert.IsTrue(wire.TerminalA != wire.TerminalB);
		}

		[Test]
		public void WiresDoNotShortCircuit()
		{
			using var wire = new Wire<byte>();
			
			wire.TerminalA.Enqueue(0x0a);
			
			Assert.IsFalse(wire.TerminalA.TryDequeue(out _));
		}

		[Test]
		public void TerminalASendsToTerminalB()
		{
			var wire = new Wire<byte>();
			
			wire.TerminalA.Enqueue(0x0a);

			Assert.IsTrue(wire.TerminalB.TryDequeue(out byte actual));
			Assert.AreEqual(0x0a, actual);
		}
		
		[Test]
		public void TerminalBSendsToTerminalA()
		{
			var wire = new Wire<byte>();
			
			wire.TerminalB.Enqueue(0x0a);

			Assert.IsTrue(wire.TerminalA.TryDequeue(out byte actual));
			Assert.AreEqual(0x0a, actual);
		}

		[Test]
		public void CanSendUsingWireStreams()
		{
			using var wire = new Wire<byte>();

			using var streamA = new WireTerminalStream(wire.TerminalA);
			using var streamB = new WireTerminalStream(wire.TerminalB);

			byte[] helloBytes = Encoding.UTF8.GetBytes("Hello world!");

			streamA.Write(helloBytes, 0, helloBytes.Length);

			var readBuffer = new byte[helloBytes.Length];
			int bytesRead = streamB.Read(readBuffer, 0, readBuffer.Length);
			
			Assert.AreEqual(bytesRead, readBuffer.Length);

			Assert.IsTrue(readBuffer.SequenceEqual(helloBytes));
		}
		
		
		
		
	}
}