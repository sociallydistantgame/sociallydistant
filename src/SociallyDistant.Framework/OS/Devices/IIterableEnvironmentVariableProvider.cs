#nullable enable
namespace SociallyDistant.Core.OS.Devices
{
	public interface IIterableEnvironmentVariableProvider : 
		IEnvironmentVariableProvider, 
		IEnumerable<KeyValuePair<string, string>>
	{
	}
}