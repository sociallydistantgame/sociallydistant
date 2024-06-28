#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core
{
	public interface IWorldFlagCollection :
		IList<string>,
		IWorldData
	{
		
	}
}