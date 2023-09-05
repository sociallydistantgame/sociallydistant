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
	public interface ISerializable<TRevision, TSerializer> 
		where TRevision : Enum
		where TSerializer : IRevisionedSerializer<TRevision>
	{
		void Serialize(TSerializer serializer);
	}

	public interface IWorldData : ISerializable<WorldRevision, IWorldSerializer>
	{
		
	}
}