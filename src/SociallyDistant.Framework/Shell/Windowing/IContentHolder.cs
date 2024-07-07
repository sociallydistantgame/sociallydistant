#nullable enable
namespace SociallyDistant.Core.Shell.Windowing
{
	public interface IContentHolder
	{
		IContentPanel? ActiveContent { get; }
	}
}