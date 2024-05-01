#nullable enable
using System;
using Modules;
using UnityEngine;
using System.Threading.Tasks;

namespace Core.Scripting.StandardModules
{
	public sealed class ShellHelpersModule : ScriptModule
	{
		private readonly IGameContext game;

		public ShellHelpersModule(IGameContext game)
		{
			this.game = game;
		}

		[Function("shellexec")]
		public int ExecuteShellAction(string uri)
		{
			if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri parsedUri))
				return 3;

			if (!game.UriManager.IsSchemeRegistered(parsedUri.Scheme))
				return 2;

			try
			{
				game.UriManager.ExecuteNavigationUri(parsedUri);
				return 0;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				return 1;
			}
		}

		[Function("save")]
		public async Task SaveGame()
		{
			await game.SaveCurrentGame(false);
		}

		[Function("error")]
		public async Task Error(string message)
		{
			await game.Shell.ShowInfoDialog("Error", message);
		} 
	}
}