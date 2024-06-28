using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Scripting.Consoles;

namespace SociallyDistant.Core.Scripting.Instructions
{
	public sealed class PipeInstruction : ShellInstruction
	{
		private readonly ShellInstruction pipeIn;
		private readonly ShellInstruction pipeOut;

		private ISystemProcess? shellProcess;
		private ITextConsole? pipeInConsole;
		private ITextConsole? pipeOutConsole;
		private bool hasInputStarted;
		private bool hasOutputStarted;


		public PipeInstruction(ShellInstruction pipeIn, ShellInstruction pipeOut)
		{
			this.pipeIn = pipeIn;
			this.pipeOut = pipeOut;
		}
		
		/// <inheritdoc />
		public override async Task<int> RunAsync(ITextConsole console, IScriptExecutionContext context)
		{
			// Pipe instructions are weird.
			//
			// We need to do something special for them.
			// 
			// If we're the first pipe in a pipe sequence, we were likely given the same
			// console device as the shell.
			//
			// If the above is the case, we need to create the sequence of pipe consoles
			// that will be used for the rest of the pipe. How we do this depends on
			// whether our output is another pipe.
			//
			// If we're just outputting into a single command, we create two pipe consoles
			// One that takes input from the REAL console, and outputs to a line list.
			// The other console reads from the line list and writes to the real console.
			//
			// If we're outputting to another pipe, we need to have both our own consoles
			// output to a different line list, and give the second console we create as well
			// as the real console to the output pipe. It's confusing as fuck.
			// 
			// Also, disregard all of this text if we already have the two consoles we need...
			if (pipeInConsole == null && pipeOutConsole == null)
			{
				// Create the first line list
				var lineList = new LineListConsole();

				// outputting to another pipe
				if (pipeOut is PipeInstruction pipeInstruction)
				{
					// first command goes to our line list
					pipeInConsole = new RedirectedConsole(console, lineList);

					// second command goes to whatever our output pipe says.
					pipeOutConsole = pipeInstruction.CreateSlavePipe(lineList, console);
				}

				// anything else
				else
				{
					// redirect first command output into the line list
					pipeInConsole = new RedirectedConsole(console, lineList);

					// redirect second command input to the line list
					pipeOutConsole = new RedirectedConsole(lineList, console);
				}
			}
			
			if (pipeInConsole == null)
				return -1;

			if (pipeOutConsole == null)
				return -1;

			await pipeIn.RunAsync(pipeInConsole, context);
			return await pipeOut.RunAsync(pipeOutConsole, context);
		}

		private ITextConsole CreateSlavePipe(ITextConsole input, ITextConsole masterOutput)
		{
			// Create a line list to send input to
			var newLineList = new LineListConsole();
			pipeInConsole = new RedirectedConsole(input, newLineList);

			if (pipeOut is PipeInstruction pipeInstruction)
			{
				pipeOutConsole = pipeInstruction.CreateSlavePipe(newLineList, masterOutput);
			}
			else
			{
				pipeOutConsole = new RedirectedConsole(newLineList, masterOutput);
			}

			return pipeInConsole;
		}
	}
}