#nullable enable
using System;
using System.Threading.Tasks;
using Architecture;
using OS.Devices;
using OS.FileSystems;
using UnityEngine;

namespace VfsMapping
{
	public sealed class ScriptableCommandFileEntry : AssetFileEntry<ScriptableCommandBase>
	{
		/// <inheritdoc />
		public override bool CanExecute => true;
		
		/// <inheritdoc />
		public ScriptableCommandFileEntry(IDirectoryEntry parent, ScriptableCommandBase asset) : base(parent, asset)
		{ }

		/// <inheritdoc />
		public override async Task<bool> TryExecute(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			try
			{
				await Asset.Main(process, console, arguments);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				process.Kill();
			}

			return true;
		}
	}
}