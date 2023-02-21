#nullable enable
namespace OS.Devices
{
	public interface IEnvironmentVariableProvider
	{
		string this[string key] { get; set; }

		IEnvironmentVariableProvider DeepClone();
	}
}