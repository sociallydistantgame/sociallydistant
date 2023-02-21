#nullable enable

using OS.Devices;
using OS.Tasks;
using UnityEngine;

namespace Architecture
{
	public abstract class CommandScript :
		MonoBehaviour,
		ICommandTask
	{
		/// <inheritdoc />
		public void Main(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			
		}
	}
}