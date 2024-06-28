#nullable enable
namespace SociallyDistant.Core.Scripting
{
	public static class CommonScriptHooks
	{
		public static readonly string BeforeWorldStateUpdate = nameof(BeforeWorldStateUpdate);
		public static readonly string AfterWorldStateUpdate = nameof(AfterWorldStateUpdate);
		public static readonly string AfterContentReload = nameof(AfterContentReload);
		public static readonly string BeforeUpdateShellTools = nameof(BeforeUpdateShellTools);
	}
}