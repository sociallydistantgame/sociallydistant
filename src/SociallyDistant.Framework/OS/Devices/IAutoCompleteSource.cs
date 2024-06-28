using System.Text;

namespace SociallyDistant.Core.OS.Devices
{
	public interface IAutoCompleteSource
	{
		IReadOnlyList<string> GetCompletions(StringBuilder text, out int insertionPoint);
	}
}