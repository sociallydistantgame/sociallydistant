#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core
{
	public interface IWorldDataObject<TDataElement> : ISerializableDataObject<TDataElement, WorldRevision, IWorldSerializer>
		where TDataElement : struct, IWorldData
	{
		
	}
}