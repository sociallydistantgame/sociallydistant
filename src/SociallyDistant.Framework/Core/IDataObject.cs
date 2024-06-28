#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core
{
	/// <summary>
	///		Represents an object that contains a single serializable world data object.
	/// </summary>
	/// <typeparam name="TDataElement">The type of data stored within the container</typeparam>
	public interface IDataObject<TDataElement>
		where TDataElement : struct
	{
		TDataElement Value { get; set; }
	}

	public interface ISerializableDataObject<TDataElement, TRevision, TSerializer> : 
		IDataObject<TDataElement>
		where TDataElement : struct, ISerializable<TRevision, TSerializer>
		where TRevision : Enum
		where TSerializer : IRevisionedSerializer<TRevision>
	{
		void Serialize(TSerializer serializer);
	}
}