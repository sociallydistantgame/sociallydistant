#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using OS.FileSystems;
using UnityEngine.Assertions;

namespace OS.Devices
{
	public class PlayerComputer : IComputer
	{
		private string hostname;
		private Dictionary<int, IUser> users = new Dictionary<int, IUser>();
		private Dictionary<string, int> usernameMap = new Dictionary<string, int>();
		private PlayerUser playerUser;
		private PlayerFileSystem playerFileSystem;

		/// <inheritdoc />
		public string Name => hostname;
		public PlayerUser PlayerUser => playerUser;
		
		public PlayerComputer(string hostname, string username)
		{
			this.hostname = hostname;

			this.AddUser(new SuperUser(this));

			this.playerUser = new PlayerUser(this, username);
			this.AddUser(this.playerUser);
			this.playerFileSystem = new PlayerFileSystem(this);
			
			
		}
		
		/// <inheritdoc />
		public bool FindUserById(int id, out IUser? user)
		{
			return users.TryGetValue(id, out user);
		}

		/// <inheritdoc />
		public bool FindUserByName(string username, out IUser? user)
		{
			user = default;
			if (!usernameMap.TryGetValue(username, out int uid))
				return false;

			return FindUserById(uid, out user);
		}

		/// <inheritdoc />
		public ISystemProcess? ExecuteProgram(ISystemProcess parentProcess, ITextConsole console, string programName, string[] arguments)
		{
			// Perhaps we should remove this in favour of calling the VFS method directly?
			return GetFileSystem(parentProcess.User)
				.Execute(parentProcess, programName, console, arguments);
		}

		/// <inheritdoc />
		public VirtualFileSystem GetFileSystem(IUser user)
		{
			return new VirtualFileSystem(this.playerFileSystem, user);
		}

		private void AddUser(IUser user)
		{
			Assert.IsTrue(user.Computer == this, "User does not belong to the player computer.");
			Assert.IsFalse(users.ContainsKey(user.Id), "User already exists on this computer");
			Assert.IsFalse(usernameMap.ContainsKey(user.UserName), "Duplicate username!");

			this.users.Add(user.Id, user);
			this.usernameMap.Add(user.UserName, user.Id);
		}
	}
}