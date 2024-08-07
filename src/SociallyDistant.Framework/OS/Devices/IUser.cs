﻿namespace SociallyDistant.Core.OS.Devices
{
	public interface IUser
	{
		int Id { get; }
		string UserName { get; }
		string Home { get; }
		PrivilegeLevel PrivilegeLevel { get; }
		IComputer Computer { get; }

		bool CheckPassword(string password);
	}
}