#nullable enable
namespace SociallyDistant.Core.Modules
{
	public interface IUriManager
	{
		IGameContext GameContext { get; }
		
		bool IsSchemeRegistered(string name);
		
		void RegisterSchema(string schemaName, IUriSchemeHandler handler);
		void UnregisterSchema(string schemaName);
		
		void ExecuteNavigationUri(Uri uri);
	}
}