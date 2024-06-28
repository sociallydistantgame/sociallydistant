#nullable enable
using SociallyDistant.Core.Social;

namespace SociallyDistant.Core.Chat
{
	public interface IBranchDefinition
	{
		IProfile Target { get; }
		string Message { get; }
		
		void Pick();
	}
}