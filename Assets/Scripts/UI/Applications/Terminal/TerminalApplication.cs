#nullable enable

using System;
using Architecture;
using OS.Devices;
using UI.Windowing;
using UnityEngine;
using Utility;

namespace UI.Applications.Terminal
{
	public class TerminalApplication :
		MonoBehaviour,
		IProgramOpenHandler
	{
		private ISystemProcess process = null!;
		private IWindow window = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TerminalApplication));
		}

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IWindow window)
		{
			this.process = process;
			this.window = window;
		}
	}
}