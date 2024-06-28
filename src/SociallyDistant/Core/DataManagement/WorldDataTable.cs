using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Serialization;
using SociallyDistant.Core.Core.Systems;

namespace SociallyDistant.Core.DataManagement
{
	public sealed class WorldDataTable<TDataElement> : 
		DataTable<TDataElement, WorldRevision, IWorldSerializer>,
		IWorldTable<TDataElement>
		where TDataElement : struct, IWorldData, IDataWithId
	{
		/// <inheritdoc />
		public WorldDataTable(UniqueIntGenerator instanceIdgenerator, DataEventDispatcher eventDispatcher, bool dispatchOnSerialize = true) : base(instanceIdgenerator, eventDispatcher, dispatchOnSerialize)
		{ }
	}
}