using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.DataManagement
{
	public sealed class WorldDataObject<TDataElement> : 
		DataObject<WorldRevision, TDataElement, IWorldSerializer>,
		IWorldDataObject<TDataElement>	
		where TDataElement : struct, IWorldData
	{
		/// <inheritdoc />
		public WorldDataObject(DataEventDispatcher eventDispatcher) : base(eventDispatcher)
		{ }
	}
}