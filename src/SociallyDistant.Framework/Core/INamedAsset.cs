#nullable enable

using SociallyDistant.Core.ContentManagement;

namespace SociallyDistant.Core.Core
{
	public interface INamedAsset : IGameContent
	{
		public string Name { get; }
	}
}