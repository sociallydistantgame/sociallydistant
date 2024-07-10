using System.Collections.Concurrent;

namespace SociallyDistant.Core.UI.Terminal;

internal class ThreadSafeFifoBuffer : Stream
{
    private readonly ConcurrentQueue<byte[]> queuedData = new ConcurrentQueue<byte[]>();
        
    private byte[] realBuffer = new byte[1024];
    private int    length;

    public override bool CanRead => true; // Yes...

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => -1;

    public override long Position
    {
        get
        {
            return -1;
        }
        set => throw new NotSupportedException(); // No.
    }

    public override void Flush()
    { }

    internal bool ThrowOnTerminationRequest { get; set; }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var bytesRead = 0;

        while (queuedData.TryDequeue(out byte[] block) && bytesRead < count)
        {
            int copyCount = Math.Min(count, block.Length);
                
            Buffer.BlockCopy(block, 0, buffer, offset + bytesRead, copyCount);

            count -= copyCount;
            bytesRead += copyCount;
        }
            
        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return -1; // No.
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException(); // No.
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        const int blockSize = 1024;
        for (int i = offset; i < count; i += blockSize)
        {
            var block = new byte[Math.Min(blockSize, count - i)];
             
            Buffer.BlockCopy(buffer, i, block, 0, block.Length);
                
            this.queuedData.Enqueue(block);
        }
    }
}