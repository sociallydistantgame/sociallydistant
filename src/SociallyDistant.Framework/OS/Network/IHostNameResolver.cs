#nullable enable
namespace SociallyDistant.Core.OS.Network
{
	/// <summary>
	///		Interface for an object capable of performing hostname lookups.
	/// </summary>
	public interface IHostNameResolver
	{
		bool IsValidSubnet(uint address);
		string? ReverseHostLookup(uint networkAddress);
		uint? HostLookup(string hostname);
	}
}