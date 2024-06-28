namespace SociallyDistant.Core.Core.Serialization
{
	public interface IRevisionedSerializer<TRevision> : IDisposable 
		where TRevision : Enum
	{
		IRevisionComparer<TRevision> RevisionComparer { get; }
		bool IsWriting { get; }
		bool IsReading { get; }

		void Serialize(ref DateTime value);
		void Serialize(ref sbyte value);
		void Serialize(ref byte value);
		void Serialize(ref short value);
		void Serialize(ref ushort value);
		void Serialize(ref int value);
		void Serialize(ref uint value);
		void Serialize(ref long value);
		void Serialize(ref ulong value);
		void Serialize(ref bool value);
		void Serialize(ref float value);
		void Serialize(ref double value);
		void Serialize(ref decimal value);
		void Serialize(ref char value);
		void Serialize(ref string value);
		void Serialize(ref ISerializable value);
	}
}