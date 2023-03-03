#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GameplaySystems.GameManagement;
using OS.FileSystems;
using OS.FileSystems.Host;
using UnityEngine.Assertions;

namespace OS.Devices
{
	public class PlayerComputer : IComputer
	{
		private readonly GameManager gameManager;
		private readonly IUser su;
		private string hostHomeDirectory = string.Empty;
		private Dictionary<int, IUser> users = new Dictionary<int, IUser>();
		private Dictionary<string, int> usernameMap = new Dictionary<string, int>();
		private PlayerUser playerUser;
		private PlayerFileSystem? playerFileSystem;

		/// <inheritdoc />
		public string Name => gameManager.CurrentPlayerHostName;
		public PlayerUser PlayerUser => playerUser;
		
		public PlayerComputer(GameManager gameManager)
		{
			this.gameManager = gameManager;

			su = new SuperUser(this);
			this.AddUser(su);

			this.playerUser = new PlayerUser(this, gameManager.CurrentPlayerName);
			this.AddUser(this.playerUser);

			this.RebuildVfs();
		}

		public void RebuildVfs()
		{
			string homeParent = gameManager.CurrentGamePath ?? gameManager.GameDataDirectory;
			string homePath = Path.Combine(homeParent, "home");
			hostHomeDirectory = homePath;

			this.playerFileSystem = new PlayerFileSystem(this);
			
			GetFileSystem(su)
				.Mount(playerUser.Home, new HostJail(hostHomeDirectory));
		}
		
		/// <inheritdoc />
		public bool FindUserById(int id, out IUser? user)
		{
			return users.TryGetValue(id, out user);
		}

		public void SetPlayerUserName(string username)
		{
			if (username == "root")
				throw new InvalidOperationException("Cannot name a non-root user 'root'.");

			this.playerUser.UserName = username;

			this.RebuildVfs();
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
			if (playerFileSystem == null)
				RebuildVfs();
			
			return new VirtualFileSystem(this.playerFileSystem!, user);
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