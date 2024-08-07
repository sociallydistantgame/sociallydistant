﻿using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Shell.Commands
{
	[CustomCommand("whoami")]
	public class WhoAmI : CustomCommand
	{
		/// <inheritdoc />
		public override Task<int> Run(string[] args, ISystemProcess process, ConsoleWrapper console)
		{
			console.WriteLine(process.User.UserName);
			return Task.FromResult(0);
		}
	}
}