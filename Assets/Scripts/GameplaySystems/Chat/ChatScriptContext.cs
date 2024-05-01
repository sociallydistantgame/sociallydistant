#nullable enable
using System;
using System.Threading.Tasks;
using Core.Scripting;
using OS.Devices;
using UnityEngine;
using System.Collections.Generic;
using Chat;

namespace GameplaySystems.Chat
{
	public sealed class ChatScriptContext : IScriptExecutionContext
	{
		private readonly Dictionary<string, string> variables = new();
		private readonly ScriptFunctionManager functions = new();
		private readonly ScriptModuleManager modules = new();
		private readonly IConversationController controller;

		/// <inheritdoc />
		public string Title => controller.Conversation.Id;

		public ChatScriptContext(IConversationController controller)
		{
			this.controller = controller;

			this.modules.RegisterModule(new ChatScriptFunctions(this.controller));
		}
		
		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			if (!variables.TryGetValue(variableName, out string value))
				return string.Empty;

			return value;
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
			variables[variableName] = value;
		}

		/// <inheritdoc />
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			int? moduleResult = await modules.TryExecuteFunction(name, args, console, callSite ?? this);
			if (moduleResult != null)
				return moduleResult;

			return await functions.CallFunction(name, args, console, callSite ?? this);
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			return realConsole;
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, ITextConsole console)
		{
			throw new InvalidOperationException($"{this.Title}: {name}: Command not found.");
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			functions.DeclareFunction(name, body);
		}
	}
}