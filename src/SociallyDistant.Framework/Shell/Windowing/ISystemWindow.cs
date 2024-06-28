#nullable enable
namespace SociallyDistant.Core.Shell.Windowing
{
	public interface ISystemWindow : IWindow
	{
		string Title { get; set; }
	}
}