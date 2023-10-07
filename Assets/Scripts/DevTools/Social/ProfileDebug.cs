using Core;

namespace DevTools.Social
{
	public class ProfileDebug : IDevMenu
	{
		private readonly WorldManager world;
			
		public ProfileDebug(WorldManager world)
		{
			this.world = world;
		}

		/// <inheritdoc />
		public string Name => "Social Profiles";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
		}
	}
}