using System;

namespace Core.Serialization
{
	public interface ISerializable<TRevision> where TRevision : Enum
	{
		void Serialize(IRevisionedSerializer<TRevision> serializer);
	}
}