#nullable enable
namespace SociallyDistant.Core.Config
{
	/// <summary>
	///		Applies to <see cref="Modules.GameModule"/> classes and allows them to be executed by the game regardless of the "Enable script mods" setting.
	///		This can only be used for system modules within Socially Distant itself or within DLC.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class IgnoreModdingLegalWaiverAttribute : Attribute
	{
		
	}
}