#nullable enable
namespace Shell.Windowing
{
	public interface ISystemWindow : IWindow
	{
		string Title { get; set; }
	}
}