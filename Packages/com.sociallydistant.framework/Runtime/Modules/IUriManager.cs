#nullable enable
using System;

namespace Modules
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