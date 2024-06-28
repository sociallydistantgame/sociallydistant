#nullable enable
using ImGuiNET;

namespace SociallyDistant.DevTools
{
	public sealed class UriRunnerMenu : IDevMenu
	{
		private readonly SociallyDistantGame gameManager = null!;
		
		private Exception? exception;
		private string rawUri = string.Empty;
		
		/// <inheritdoc />
		public string Name => "Run Shell URI";

		internal UriRunnerMenu(SociallyDistantGame game)
		{
			this.gameManager = game;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (exception != null)
			{
				ImGui.Text("URI Handler Error");
				ImGui.Text(exception.ToString());

				if (ImGui.Button("Dismiss"))
					exception = null;
				
				return;
			}
			
			ImGui.Text("Enter Shell URI:");

			if (ImGui.Button("Run"))
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