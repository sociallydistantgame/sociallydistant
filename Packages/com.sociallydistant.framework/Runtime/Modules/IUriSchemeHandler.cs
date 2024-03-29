#nullable enable
using System;

namespace Modules
{
	public interface IUriSchemeHandler
	{
		void HandleUri(Uri uri);
	}
}