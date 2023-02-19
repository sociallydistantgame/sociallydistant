#nullable enable

using System;
using Architecture;
using OS.Devices;
using UI.Terminal.SimpleTerminal;
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
		private SimpleTerminalRenderer st = null!;
		private ITextConsole textConsole;
		private bool isWaitingForInput;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TerminalApplication));
			this.MustGetComponentInChildren(out st);
		}

		private void Start()
		{
			this.textConsole = st.StartSession();
		}

		private void Update()
		{
			if (!isWaitingForInput)
			{
				textConsole.WriteText("write text> ");
				isWaitingForInput = true;
			}
			else
			{
				if (textConsole.TryDequeueSubmittedInput(out string line))
				{
					textConsole.WriteText(line + Environment.NewLine);
					isWaitingForInput = false;
				}
			}
		}

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IWindow window)
		{
			this.process = process;
			this.window = window;
		}
	}
}