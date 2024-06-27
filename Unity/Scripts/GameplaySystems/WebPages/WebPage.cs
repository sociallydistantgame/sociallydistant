using AcidicGui.Mvc;

namespace GameplaySystems.WebPages
{
	public abstract class WebPage : View
	{
		public string Path { get; private set; } = "/";
	}
}