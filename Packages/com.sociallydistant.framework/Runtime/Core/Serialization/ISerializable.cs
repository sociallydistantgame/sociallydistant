using System;

namespace Core.Serialization
{
#nullable enable
	public interface ISerializable
	{
		void Write(IDataWriter writer);
		void Read(IDataReader reader);
	}

#nullable restore
	public interface ISerializable<TRevision> where TRevision : Enum
	{
		void Serialize(IRevisionedSerializer<TRevision> serializer);
	}
}