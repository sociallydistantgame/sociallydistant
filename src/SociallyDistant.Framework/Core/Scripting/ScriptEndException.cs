#nullable enable

namespace SociallyDistant.Core.Core.Scripting
{
	public sealed class ScriptEndException : Exception
	{
		public int ExitCode { get; private set; }
		public bool LocalScope { get; private set; }
		
		public ScriptEndException(int exitCode, bool localScope)
			: base($"A script instruction has requested execution to halt. Exit code: {exitCode}")
		{
			this.ExitCode = exitCode;
			this.LocalScope = localScope;
		}
	}
}