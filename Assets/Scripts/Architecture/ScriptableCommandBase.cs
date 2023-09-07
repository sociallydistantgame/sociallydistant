#nullable enable
using Core;
using OS.Devices;
using OS.Tasks;
using UnityEngine;
using System.Threading.Tasks;

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
		public abstract Task Main(ISystemProcess process, ITextConsole console, string[] arguments);
	}
}