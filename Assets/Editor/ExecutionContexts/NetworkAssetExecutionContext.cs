#nullable enable

using System;
using System.Threading.Tasks;
using Core.Scripting;
using GameplaySystems.Hacking.Assets;
using OS.Devices;
using OS.Network;
using UnityEngine;

namespace Editor.ExecutionContexts
{
	public class NetworkAssetExecutionContext : IScriptExecutionContext
	{
		private readonly NetworkAsset networkAsset;
		private readonly ScriptFunctionManager functions = new();

		public NetworkAssetExecutionContext(NetworkAsset asset)
		{
			this.networkAsset = asset;
		}

		/// <inheritdoc />
		public string Title => "Network";

		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			int? functionResult = await functions.CallFunction(name, args, console, callSite ?? this);
			if (functionResult != null)
				return functionResult;
			
			bool result = TryExecuteBuiltin(name, args);

			return result ? 0 : null;
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			if (mode != FileRedirectionType.None)
				Debug.LogWarning("File redirection is not supported in editor shell scripts.");
			
			return realConsole;
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, ITextConsole console)
		{
			throw new InvalidOperationException($"Command not found: {name}");
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			functions.DeclareFunction(name, body);
		}

		private void CheckArgs(string[] args, int expectedLength, string error)
		{
			if (args.Length < expectedLength)
				throw new InvalidOperationException(error);

			for (var i = 0; i < expectedLength; i++)
			{
				if (string.IsNullOrWhiteSpace(args[i]))
					throw new InvalidOperationException(error);
			}
		}

		private void ThrowIfNarrativeIdIsSet()
		{
			if (string.IsNullOrEmpty(networkAsset.NarrativeId))
				return;

			throw new InvalidOperationException("You can only set the narrative ID of a network once per script!");
		}

		private void ThrowIfNarrativeIdIsNotSet()
		{
			if (!string.IsNullOrEmpty(networkAsset.NarrativeId))
				return;

			throw new InvalidOperationException("You must set the narrative ID for the network before you can add any data to it.");
		}

		private void ThrowIfNameIsSet()
		{
			if (string.IsNullOrEmpty(networkAsset.NetworkName))
				return;

			throw new InvalidOperationException("Network name has already been set!");
		}
		
		private void ThrowIfNameNotSet()
		{
			if (!string.IsNullOrEmpty(networkAsset.NetworkName))
				return;

			throw new InvalidOperationException("Network name and narrative ID must be set before performing this operation.");
		}

		private void ParseForwardRule(string deviceString, out ushort outerPort, out string deviceName, out ushort port)
		{
			string[] delimited = deviceString.Split(':', StringSplitOptions.RemoveEmptyEntries);
			CheckArgs(delimited, 3, "port forward string parse error, expected format: <outerPort>:<narrativeId>:<port>");

			if (!ushort.TryParse(delimited[0], out outerPort))
				throw new InvalidOperationException("Outside Port could not be parsed into a 16-bit unsigned integer. Port is invalid.");
			
			deviceName = delimited[1];

			if (!ushort.TryParse(delimited[2], out port))
				throw new InvalidOperationException("Device Port could not be parsed into a 16-bit unsigned integer. Port is invalid.");
		}
		
		private void ParseDeviceString(string deviceString, out string deviceName, out ushort port)
		{
			string[] delimited = deviceString.Split(':', StringSplitOptions.RemoveEmptyEntries);
			CheckArgs(delimited, 2, "device string parse error, expected format: <narrativeId>:<port>");

			deviceName = delimited[0];

			if (!ushort.TryParse(delimited[1], out port))
				throw new InvalidOperationException("Port could not be parsed into a 16-bit unsigned integer. Port is invalid.");
		}
		
		private bool TryExecuteBuiltin(string name, string[] args)
		{
			switch (name)
			{
				case "id":
				{
					CheckArgs(args, 1, "usage: id <narrativeID>");
					ThrowIfNarrativeIdIsSet();

					networkAsset.SetNarrativeId(args[0]);

					return true;
				}
				case "name":
				{
					CheckArgs(args, 1, "usage: name <friendly name>");
					ThrowIfNarrativeIdIsNotSet();
					ThrowIfNameIsSet();

					networkAsset.SetFriendlyName(string.Join(" ", args));
					
					return true;
				}
				case "isp":
				{
					CheckArgs(args, 1, "usage: isp <ispId>");
					ThrowIfNameNotSet();

					networkAsset.SetIsp(args[0]);
					
					return true;
				}
				case "domain":
				{
					CheckArgs(args, 1, "Domain name expected");
					ThrowIfNameNotSet();

					networkAsset.AddDomainName(args[0]);
					
					return true;
				}
				case "subnet":
				{
					CheckArgs(args, 1, "CIDR-formatted subnet expected.");
					ThrowIfNameNotSet();

					string cidrString = args[0];

					if (!NetUtility.TryParseCidrNotation(cidrString, out Subnet subnet))
						throw new InvalidOperationException("Subnet could not be parsed as a valid CIDR subnet string!");

					networkAsset.SetSubnet(subnet);
					return true;
				}
				case "host":
				{
					CheckArgs(args, 2, "Usage: host <narrativeID> <hostname>");
					ThrowIfNameNotSet();

					string narrativeId = args[0];
					string hostName = args[1];

					networkAsset.AddDevice(narrativeId, hostName);
					return true;
				}
				case "listen":
				{
					CheckArgs(args, 2, "usage: listen <device>:<port> <serverType>");
					ThrowIfNameNotSet();

					string deviceString = args[0];
					string rawServerType = args[1];

					ParseDeviceString(deviceString, out string narrativeId, out ushort port);

					if (!Enum.TryParse(rawServerType, out ServerType serverType))
						throw new InvalidOperationException("Server type is invalid");

					networkAsset.AddListener(narrativeId, port, serverType);
					return true;
				}
				case "forward":
				{
					CheckArgs(args, 1, "usage: forward <publicPort>:<device>:<devicePort>");
					ThrowIfNameNotSet();

					ParseForwardRule(args[0], out ushort outsidePort, out string deviceId, out ushort insidePort);
					networkAsset.PortForward(outsidePort, deviceId, insidePort);
					return true;
				}
				case "proxy":
				{
					CheckArgs(args, 1, "Usage: proxy <proxyServer>");
					ThrowIfNameNotSet();
					
                    string serverName = args[0];
					
					networkAsset.SetProxyDevice(serverName);
					return true;
				}
				case "proxypass":
				{
					CheckArgs(args, 2, "Usage: proxypass <domain> <server>");
					ThrowIfNameNotSet();

					string domain = args[0];
					string server = args[1];
					
					networkAsset.ProxyForward(domain, server);
					
					return true;
				}
				case "user":
				{
					CheckArgs(args, 4, "usage: user <device> <username> <type> <authLevel>");
					ThrowIfNameNotSet();

					string device = args[0];
					string username = args[1];
					string rawType = args[2];
					string rawAuthLevel = args[3];

					NetworkAsset.UserType userType = rawType switch
					{
						"admin" => NetworkAsset.UserType.Admin,
						"limited" => NetworkAsset.UserType.Limited,
						_ => throw new InvalidOperationException("User type must be either 'admin' or 'limited'.")
					};

					// TODO: Auth levels
					networkAsset.AddUser(device, username, userType);
					return true;
				}
			}
			
			return false;
		}
	}
}