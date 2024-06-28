namespace SociallyDistant.Core.Shell.Windowing
{
	public interface IWindowCloseBlocker
	{
		bool CheckCanClose();
	}
}