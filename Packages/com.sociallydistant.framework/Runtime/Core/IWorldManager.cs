#nullable enable

namespace Core
{
	public interface IWorldManager
	{
		IWorld World { get; }
		
		IWorldDataCallbacks Callbacks { get; }
		
		ObjectId GetNextObjectId();
	}
}