#nullable enable

using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core
{
	public interface IWorldManager
	{
		IWorld World { get; }
		
		IWorldDataCallbacks Callbacks { get; }
		
		ObjectId GetNextObjectId();
		string GetNextIspRange();
		uint GetNextPublicAddress(ObjectId ispId);
		void SaveWorld(IDataWriter writer);
	}
}