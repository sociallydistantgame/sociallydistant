#nullable enable

using ContentManagement;

namespace Core
{
	public interface INamedAsset : IGameContent
	{
		public string Name { get; }
	}
}