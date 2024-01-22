#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using Architecture;
using OS.Devices;
using UnityEngine;

namespace Core.Scripting
{
	[CreateAssetMenu(menuName = "ScriptableObject/Shell Scripting/Device Script Execution Context")]
	public sealed class DeviceUpdateScriptContext : ScriptExecutionContext
	{
		[SerializeField]
		private DeviceCoordinator deviceCoordinator;

		/// <inheritdoc />
		public override async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			// Prioritize commands from the base ScriptExecution context over ones on the device.
			int? baseResult = await base.TryExecuteCommandAsync(name, args, console, callSite);
			if (baseResult != null)
				return baseResult;

			if (deviceCoordinator == null)
				throw new InvalidOperationException("The DeviceCoordinator has not been assigned to the DeviceScriptExecutionContext.");

			ISystemProcess? targetProcess = null;
			string? narrativeId = callSite?.GetVariableValue("SD_INTERNAL_COMPUTER_NARRATIVE_ID");
			if (string.IsNullOrWhiteSpace(narrativeId))
				targetProcess = deviceCoordinator.GetAllRootTasks().FirstOrDefault(x => x.User.Computer is PlayerComputer);
			
			// TODO: Computers do not yet have narrative identifiers.

			if (targetProcess == null)
				throw new InvalidOperationException($"Cannot find an active device with the narrative identifier: {narrativeId}. Has the device been spawned in?");

			var systemContext = new OperatingSystemExecutionContext(targetProcess);

			return await systemContext.TryExecuteCommandAsync(name, args, console, callSite);
		}
	}
}