using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SociallyDistant.Core.ContentManagement;

public sealed class XnbContentIdentifier : IDisposable
{
    private static readonly Type lzxDecoderStreamType = typeof(Microsoft.Xna.Framework.Content.ContentManager)
        .Assembly.GetType("MonoGame.Framework.Utilities.LzxDecoderStream")!;
    private static readonly Type lz4DecoderStreamType = typeof(Microsoft.Xna.Framework.Content.ContentManager)
        .Assembly.GetType("MonoGame.Framework.Utilities.Lz4DecoderStream")!;
    
    private static readonly byte[] expectedFormatIdentifier = new byte[]
    {
        (byte)'X',
        (byte)'N',
        (byte)'B'
    };
    
    private readonly Stream xnbStream;
    private readonly BinaryReader reader;

    public XnbContentIdentifier(Stream stream)
    {
        this.xnbStream = stream;
        this.reader = new BinaryReader(xnbStream, Encoding.UTF8, true);
    }

    public void Dispose()
    {
        xnbStream.Dispose();
        reader.Dispose();
    }

    public Type[] IdentifyContainedTypes()
    {
        HeaderInformation header = ReadHeader();

        Stream decoderStream = OpenContentDecoder(header);

        var types = new List<Type>();
        using (var decodedStreamReader = new BinaryReader(decoderStream, Encoding.UTF8, true))
        {
            sbyte readerCount = decodedStreamReader.ReadSByte();

            for (int i = 0; i < readerCount; i++)
            {
                string readerTypeName = decodedStreamReader.ReadString();
                
                // Skip the version, it's irrelevant.
                decodedStreamReader.ReadInt32();

                Type? contentType = GetContentType(readerTypeName);
                if (contentType == null)
                    continue;
                
                types.Add(contentType);
            }
        }
        

        if (decoderStream != this.xnbStream)
            decoderStream.Dispose();

        return types.ToArray();
    }

    private Type? GetContentType(string readerTypeName)
    {
        Type? readerType = Type.GetType(readerTypeName);

        if (readerType == null)
        {
            // Try again, with a name-only lookup.
            string name = readerTypeName.Split(", ").First();
            readerType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(x => x.GetType(name))
                .FirstOrDefault(x => x != null);
            
            // Still null? We're fucked.
            if (readerType == null)
                return null;
        }

        Type? readerBase = readerType.BaseType;
        if (readerBase == null)
            return null;

        if (readerBase.GenericTypeArguments.Length != 1)
            return null;

        return readerBase.GenericTypeArguments[0];
    }
    
    private HeaderInformation ReadHeader()
    {
        byte[] formatIdentifier = reader.ReadBytes(3);
        if (!formatIdentifier.SequenceEqual(expectedFormatIdentifier))
            throw new FormatException("The data in the XNB stream does not appear to be an actual XNB file.");

        HeaderInformation header = new();

        header.PlatformIdentifier = reader.ReadByte();
        header.FormatVersion = reader.ReadByte();
        header.Flags = (XnbFlags)reader.ReadByte();
        header.CompressedSize = reader.ReadUInt32();

        if (header.Flags.HasFlag(XnbFlags.LzxCompressed) || header.Flags.HasFlag(XnbFlags.Lz4Compressed))
            header.DecompressedSize = reader.ReadUInt32();
        else
            header.DecompressedSize = header.CompressedSize;
        
        return header;
    }

    private Stream OpenContentDecoder(HeaderInformation header)
    {
        if (header.Flags.HasFlag(XnbFlags.Lz4Compressed))
        {
            return (Stream) Activator.CreateInstance(lz4DecoderStreamType, new object[] { xnbStream })!;
        }
        else if (header.Flags.HasFlag(XnbFlags.LzxCompressed))
        {
            return (Stream)Activator.CreateInstance(lzxDecoderStreamType,
                new object[] { xnbStream, (int)header.DecompressedSize, (int)header.CompressedSize - 14 })!;
        }

        return xnbStream;
    }

    private struct HeaderInformation
    {
        public byte PlatformIdentifier;
        public byte FormatVersion;
        public XnbFlags Flags;
        public uint CompressedSize;
        public uint DecompressedSize;
    }

    [Flags]
    private enum XnbFlags : byte
    {
        None = 0,
        HiDefProfile = 0x01,
        Lz4Compressed = 64,
        LzxCompressed = 128,
        Compressed = LzxCompressed | Lz4Compressed
    }
}