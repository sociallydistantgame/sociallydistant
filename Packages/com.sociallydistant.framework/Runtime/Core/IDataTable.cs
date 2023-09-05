#nullable enable
namespace Core
{
	public interface IDataTable<TDataElement>
		where TDataElement : struct, IDataWithId
	{
		TDataElement this[ObjectId id] { get; }

		void Add(TDataElement data);

		void Remove(TDataElement data);

		void Modify(TDataElement data);

		TDataElement[] ToArray();
	}
}