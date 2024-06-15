#nullable enable
using System;
using System.Threading.Tasks;
using Core.Scripting;
using Missions;
using OS.Devices;
using UI.Tweening.Base;

namespace GameplaySystems.Missions
{
	public sealed class ObjectiveScriptContext : IScriptExecutionContext
	{
		private ScriptFunctionManager functions = new();
		private IScriptExecutionContext underlyingContext;
		private IObjectiveHandle objectiveHandle;

		public ObjectiveScriptContext(IScriptExecutionContext context, IObjectiveHandle handle)
		{
			this.underlyingContext = context;
			this.objectiveHandle = handle;
		}

		/// <inheritdoc />
		public string Title => objectiveHandle.Name;

		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			return this.underlyingContext.GetVariableValue(variableName);
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
			this.underlyingContext.SetVariableValue(variableName, value);
		}

		/// <inheritdoc />
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			int? functionResult = await functions.CallFunction(name, args, console, callSite ?? this);
			if (functionResult != null)
				return functionResult;
			
			switch (name)
			{
				case "name":
				{
					objectiveHandle.Name = string.Join(" ", args);
					return 0;
				}
				case "description":
				{
					objectiveHandle.Description = string.Join(" ", args);
					return 0;
				}
				case "complete":
				{
					objectiveHandle.MarkCompleted();
					return 0;
				}
				case "fail":
				{
					string reason = string.Join(" ", args);
					if (string.IsNullOrWhiteSpace(reason))
						reason = "Objective failed.";

					objectiveHandle.MarkFailed(reason);
					return 0;
				}
				case "hint":
				{
					objectiveHandle.Hint = string.Join(" ", args);
					return 0;
				}
			}
			
			return await underlyingContext.TryExecuteCommandAsync(name, args, console, this);
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			return underlyingContext.OpenFileConsole(realConsole, filePath, mode);
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, string[] args, ITextConsole console)
		{
			underlyingContext.HandleCommandNotFound(name, args, console);
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			functions.DeclareFunction(name, body);
		}
	}
}