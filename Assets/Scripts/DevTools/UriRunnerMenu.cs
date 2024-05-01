#nullable enable
using System;
using GamePlatform;
using UnityEngine;

namespace DevTools
{
	public sealed class UriRunnerMenu : IDevMenu
	{
		private readonly GameManager gameManager = null!;
		
		private Exception? exception;
		private string rawUri = string.Empty;
		
		/// <inheritdoc />
		public string Name => "Run Shell URI";

		public UriRunnerMenu(GameManager game)
		{
			this.gameManager = game;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (exception != null)
			{
				GUILayout.Label("URI Handler Error");
				GUILayout.Label(exception.ToString());

				if (GUILayout.Button("Dismiss"))
					exception = null;
				
				return;
			}
			
			GUILayout.Label("Enter Shell URI:");
			rawUri = GUILayout.TextField(rawUri);

			if (GUILayout.Button("Run"))
			{
				try
				{
					if (!Uri.TryCreate(rawUri, UriKind.Absolute, out Uri uri))
						throw new FormatException("Cannot parse URI");

					gameManager.UriManager.ExecuteNavigationUri(uri);
				}
				catch (Exception e)
				{
					exception = e;
				}
			}
				    
		}
	}
}