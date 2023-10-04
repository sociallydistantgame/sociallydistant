using System;

namespace Core.Serialization
{
#nullable enable
	public interface ISerializable
	{
		void Write(IDataWriter writer);
		void Read(IDataReader reader);
	}
    
	public interface ISerializable<TRevision, in TSerializer> 
		where TRevision : Enum
		where TSerializer : IRevisionedSerializer<TRevision>
	{
		void Serialize(TSerializer serializer);
	}
}