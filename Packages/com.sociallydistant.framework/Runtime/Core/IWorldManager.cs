#nullable enable

using OS.Network;

namespace Core
{
	public interface IWorldManager
	{
		IWorld World { get; }
		
		IWorldDataCallbacks Callbacks { get; }
		
		ObjectId GetNextObjectId();
		string GetNextIspRange();
		uint GetNextPublicAddress(ObjectId ispId);
	}
}