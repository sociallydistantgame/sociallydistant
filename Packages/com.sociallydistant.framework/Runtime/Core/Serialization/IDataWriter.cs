#nullable enable

using System;

namespace Core.Serialization
{
	public interface IDataWriter : IDisposable
	{
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
		void Write(DateTime dateTime);
	}
}