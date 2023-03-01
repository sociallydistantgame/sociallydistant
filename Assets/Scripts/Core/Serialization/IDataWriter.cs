#nullable enable

using Codice.CM.WorkspaceServer.Tree;

namespace Core.Serialization
{
	public interface IDataWriter
	{
		void Dispose();
		void Write(sbyte value);
		void Write(byte value);
		void Write(short value);
		void Write(ushort value);
		void Write(int value);
		void Write(uint value);
		void Write(long value);
		void Write(ulong value);
		void Write(bool value);
		void Write(float value);
		void Write(double value);
		void Write(decimal value);
		void Write(char value);
		void Write(string value);
	}

	public interface ISerializable
	{
		void Write(IDataWriter writer);
		void Read(IDataReader reader);
	}
}