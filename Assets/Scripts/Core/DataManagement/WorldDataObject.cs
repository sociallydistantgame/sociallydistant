using Core.Serialization;

namespace Core.DataManagement
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