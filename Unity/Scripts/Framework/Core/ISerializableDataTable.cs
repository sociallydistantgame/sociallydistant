#nullable enable
using System;
using Core.Serialization;

namespace Core
{
	public interface ISerializableDataTable<TDataElement, TRevision, TSerializer>
		: IDataTable<TDataElement>
		where TRevision : Enum
		where TSerializer : IRevisionedSerializer<TRevision>
		where TDataElement : struct, IDataWithId, ISerializable<TRevision, TSerializer>
	{
		void Serialize(TSerializer serializer, TRevision revision);
	}
}