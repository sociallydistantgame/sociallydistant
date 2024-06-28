#nullable enable
namespace SociallyDistant.Core.Core
{
	public interface IDataTable<TDataElement> :
		IEnumerable<TDataElement>
		where TDataElement : struct, IDataWithId
	{
		TDataElement this[ObjectId id] { get; }

		void Add(TDataElement data);

		void Remove(TDataElement data);

		void Modify(TDataElement data);

		TDataElement[] ToArray();

		bool ContainsId(ObjectId id);
	}
}