#nullable enable

namespace SociallyDistant.Core.Shell
{
	public interface IShellContext
	{
		Task ShowInfoDialog(string title, string message);
	}
}