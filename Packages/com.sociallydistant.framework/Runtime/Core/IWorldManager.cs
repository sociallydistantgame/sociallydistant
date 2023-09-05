#nullable enable

namespace Core
{
	public interface IWorldManager
	{
		IWorld World { get; }
		
		IWorldDataCallbacks Callbacks { get; }
		
		ObjectId GetNextObjectId();
	}

	/// <summary>
	///		Represents a Socially Distant world.
	/// </summary>
	public interface IWorld
	{
		
	}
}