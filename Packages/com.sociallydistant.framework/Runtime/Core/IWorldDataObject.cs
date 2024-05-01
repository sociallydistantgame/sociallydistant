#nullable enable
using Core.Serialization;

namespace Core
{
	public interface IWorldDataObject<TDataElement> : ISerializableDataObject<TDataElement, WorldRevision, IWorldSerializer>
		where TDataElement : struct, IWorldData
	{
		
	}
}