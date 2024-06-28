#nullable enable
namespace SociallyDistant.Core.OS.Network
{
	public struct PortScanResult
	{
		public readonly ushort Port;
		public readonly PortStatus Status;
		public readonly ServerType ServerType;

		public PortScanResult(ushort port, PortStatus status, ServerType serverType)
		{
			this.Port = port;
			this.Status = status;
			this.ServerType = serverType;
		}
	}
}