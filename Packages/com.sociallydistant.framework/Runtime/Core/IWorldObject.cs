#nullable enable
namespace Core
{
	/// <summary>
	///		Represents an object that contains a single serializable world data object.
	/// </summary>
	/// <typeparam name="TDataElement">The type of data stored within the container</typeparam>
	public interface IWorldObject<TDataElement>
		where TDataElement : struct
	{
		TDataElement Value { get; set; }
	}
}