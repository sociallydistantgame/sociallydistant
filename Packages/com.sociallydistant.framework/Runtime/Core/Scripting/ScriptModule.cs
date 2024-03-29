#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using OS.Devices;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using Codice.CM.SEIDInfo;

namespace Core.Scripting
{
	public abstract class ScriptModule
	{
		private readonly Dictionary<string, IScriptFunction> functions = new();

		public ScriptModule()
		{
			RegisterReflectionModules();
		}
		
		protected void DeclareFunction(string name, IScriptFunction function)
		{
			this.functions[name] = function;
		}

		public async Task<int?> TryExecuteFunction(string name, string[] args, ITextConsole console, IScriptExecutionContext context)
		{
			if (!functions.TryGetValue(name, out IScriptFunction function))
				return null;

			return await function.ExecuteAsync(name, args, console, context);
		}

		private void RegisterReflectionModules()
		{
			Type type = this.GetType();

			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Concat(type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance));

			foreach (MethodInfo method in methods)
			{
				FunctionAttribute? attribute = method.GetCustomAttributes(false)
					.OfType<FunctionAttribute>()
					.FirstOrDefault();

				if (attribute == null)
					continue;
				
				// Supported return types are:
				// - void
				// - Task
				// - string
				// - int
				// - Task<string>
				// - Task<int>
				// anything else can fuck off.
				//
				// for parameters,, we only support strings and string arrays.
				// Only the last parameter may be a string array, and will get the
				// shell arguments not passed to previous parameters.
				Type returnType = method.ReturnType;

				ParameterInfo[] parameters = method.GetParameters();

				var stringCount = 0;
				var reachedArray = false;
				var isValidParameterList = true;

				foreach (ParameterInfo param in parameters)
				{
					if (reachedArray)
					{
						isValidParameterList = false;
						break;
					}

					if (param.ParameterType == typeof(string[]))
					{
						reachedArray = true;
						continue;
					}

					if (param.ParameterType == typeof(string))
					{
						stringCount++;
						continue;
					}

					isValidParameterList = false;
					break;
				}

				// TODO: Log this.
				if (!isValidParameterList)
					continue;

				ScriptDelegate? scriptDelegate = returnType switch
				{
					_ when returnType == typeof(void) => (name, args, console, context) =>
					{
						ThrowIfTooFewArguments(name, stringCount, args);

						string[] formal = args.Take(stringCount).ToArray();
						string[] optional = args.Skip(stringCount).ToArray();

						InvokeMethod(method, formal, optional, reachedArray);

						return Task.FromResult<int>(0);
					},
					_ when returnType == typeof(int) => (name, args, console, context) =>
					{
						ThrowIfTooFewArguments(name, stringCount, args);

						string[] formal = args.Take(stringCount).ToArray();
						string[] optional = args.Skip(stringCount).ToArray();

						var result = (int) InvokeMethod(method, formal, optional, reachedArray);

						return Task.FromResult<int>(result);
					},
					_ when returnType == typeof(string) => (name, args, console, context) =>
					{
						ThrowIfTooFewArguments(name, stringCount, args);

						string[] formal = args.Take(stringCount).ToArray();
						string[] optional = args.Skip(stringCount).ToArray();

						var result = (string) InvokeMethod(method, formal, optional, reachedArray);
						console.WriteText(result);

						return Task.FromResult<int>(0);
					},
					_ when returnType == typeof(Task) => async (name, args, console, context) =>
					{
						ThrowIfTooFewArguments(name, stringCount, args);

						string[] formal = args.Take(stringCount).ToArray();
						string[] optional = args.Skip(stringCount).ToArray();

						var task = (Task) InvokeMethod(method, formal, optional, reachedArray);
						await task;
						return 0;
					},
					_ when returnType == typeof(Task<int>) => async (name, args, console, context) =>
					{
						ThrowIfTooFewArguments(name, stringCount, args);

						string[] formal = args.Take(stringCount).ToArray();
						string[] optional = args.Skip(stringCount).ToArray();

						var task = (Task<int>) InvokeMethod(method, formal, optional, reachedArray);
						var result = await task;

						return result;
					},
					_ when returnType == typeof(Task<string>) => async (name, args, console, context) =>
					{
						ThrowIfTooFewArguments(name, stringCount, args);

						string[] formal = args.Take(stringCount).ToArray();
						string[] optional = args.Skip(stringCount).ToArray();

						var task = (Task<string>) InvokeMethod(method, formal, optional, reachedArray);
						string text = await task;

						console.WriteText(text);
						return 0;
					},
					_ => null,
				};

				if (scriptDelegate == null)
					continue;

				this.DeclareFunction(attribute.Name, new ScriptDelegateFunction(scriptDelegate));
			}
		}

		private object InvokeMethod(MethodInfo method, string[] formalArgs, string[] optionalArgs, bool useOptionalArray)
		{
			var args = new object[useOptionalArray ? formalArgs.Length + 1 : formalArgs.Length];

			for (var i = 0; i < formalArgs.Length; i++)
				args[i] = formalArgs[i];

			if (useOptionalArray)
				args[^1] = optionalArgs;

			return method.Invoke(this, args);
		}
		
		private void ThrowIfTooFewArguments(string name, int expected, string[] provided)
		{
			if (provided.Length < expected)
				throw new InvalidOperationException($"{name}: Too few arguments, {expected} expected.");
		}
	}

	public delegate Task<int> ScriptDelegate(string name, string[] args, ITextConsole console, IScriptExecutionContext context);
	
	public sealed class ScriptDelegateFunction : IScriptFunction
	{
		private readonly ScriptDelegate scriptDelegate;

		public ScriptDelegateFunction(ScriptDelegate scriptDelegate)
		{
			this.scriptDelegate = scriptDelegate;
		}
		
		/// <inheritdoc />
		public async Task<int> ExecuteAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext callSite)
		{
			return await scriptDelegate(name, args, console, callSite);
		}
	}
}