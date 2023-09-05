#nullable enable
namespace Core
{
	/// <summary>
	///		Represents a serializable table of world data objects.
	/// </summary>
	/// <typeparam name="TDataElement">The type of object contained within the data table</typeparam>
	public interface IWorldTable<TDataElement>
		where TDataElement : struct, IDataWithId
	{
		TDataElement this[ObjectId id] { get; }

		void Add(TDataElement data);

		void Remove(TDataElement data);

		void Modify(TDataElement data);

		TDataElement[] ToArray();
	}
}