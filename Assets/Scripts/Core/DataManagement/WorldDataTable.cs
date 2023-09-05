using Core.Serialization;
using Core.Systems;

namespace Core.DataManagement
{
	public sealed class WorldDataTable<TDataElement> : 
		DataTable<TDataElement, WorldRevision, IWorldSerializer>,
		IWorldTable<TDataElement>
		where TDataElement : struct, IWorldData, IDataWithId
	{
		/// <inheritdoc />
		public WorldDataTable(UniqueIntGenerator instanceIdgenerator, DataEventDispatcher eventDispatcher) : base(instanceIdgenerator, eventDispatcher)
		{ }
	}
}