#nullable enable
namespace Core.Scripting
{
	public static class CommonScriptHooks
	{
		public static readonly string BeforeWorldStateUpdate = nameof(BeforeWorldStateUpdate);
		public static readonly string AfterWorldStateUpdate = nameof(AfterWorldStateUpdate);
	}
}