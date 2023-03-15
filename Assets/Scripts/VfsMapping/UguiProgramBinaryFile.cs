#nullable enable
using Architecture;
using OS.Devices;
using OS.FileSystems;
using UI.Shell;
using UnityEngine;

namespace VfsMapping
{
	public sealed class UguiProgramBinaryFile : AssetFileEntry<UguiProgram>
	{
		/// <inheritdoc />
		public override bool CanExecute => true;
		
		/// <inheritdoc />
		public UguiProgramBinaryFile(IDirectoryEntry parent, UguiProgram asset) : base(parent, asset)
		{ }

		/// <inheritdoc />
		public override bool TryExecute(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			process.Name = Name;
			
			var desktop = Object.FindObjectOfType<Desktop>();
			if (desktop == null)
			{
				console.WriteText($"{Name}: Cannot connect to X server.");
				process.Kill();
				return true;
			}

			desktop.OpenProgram(Asset, arguments, process, console);
			return true;
		}
	}
}