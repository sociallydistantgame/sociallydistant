#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Architecture
{
	public abstract class ScriptableCommandBase :
		INamedAsset,
		ICommandTask
	{
		
		private string binaryName = string.Empty;

		/// <inheritdoc />
		public string Name => binaryName;

		/// <inheritdoc />
		public abstract Task Main(ISystemProcess process, ITextConsole console, string[] arguments);
	}
}