#nullable enable
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;

namespace SociallyDistant.Core.Social
{
	public interface ICharacterGenerator : IGameContent
	{
		Task GenerateNpcs(IWorldManager world);
	}
}