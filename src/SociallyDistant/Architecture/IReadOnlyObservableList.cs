#nullable enable

namespace SociallyDistant.Architecture
{
	public interface IReadOnlyObservableList<T> : IReadOnlyList<T>
	{
		event Action<T>? ItemAdded;
		event Action<T>? ItemRemoved;
	}
}