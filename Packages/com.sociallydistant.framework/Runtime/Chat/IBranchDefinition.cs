#nullable enable
using Social;

namespace Chat
{
	public interface IBranchDefinition
	{
		IProfile Target { get; }
		string Message { get; }
		
		void Pick();
	}
}