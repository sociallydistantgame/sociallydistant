#nullable enable
namespace SociallyDistant.Core.Modules
{
	public interface IUriSchemeHandler
	{
		void HandleUri(Uri uri);
	}
}