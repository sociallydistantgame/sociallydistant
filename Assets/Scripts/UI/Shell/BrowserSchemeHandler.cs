#nullable enable
using System;
using Modules;

namespace UI.Shell
{
	public sealed class BrowserSchemeHandler : IUriSchemeHandler
	{
		private readonly Desktop shell;

		public BrowserSchemeHandler(Desktop shell)
		{
			this.shell = shell;
		}
		
		/// <inheritdoc />
		public void HandleUri(Uri uri)
		{
			// switch to (or open) the web browser in the Main Tile
			shell.OpenWebBrowser(uri);
		}
	}
}