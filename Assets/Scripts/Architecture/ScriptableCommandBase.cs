#nullable enable
using OS.Devices;
using OS.Tasks;
using UnityEngine;

namespace Architecture
{
	public abstract class ScriptableCommandBase :
		ScriptableObject,
		INamedAsset,
		ICommandTask
	{
		[SerializeField]
		private string binaryName = string.Empty;

		/// <inheritdoc />
		public string Name => binaryName;

		/// <inheritdoc />
		public abstract void Main(ISystemProcess process, ITextConsole console, string[] arguments);
	}
}