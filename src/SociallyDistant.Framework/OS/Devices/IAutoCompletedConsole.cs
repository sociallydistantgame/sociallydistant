namespace SociallyDistant.Core.OS.Devices
{
	public interface IAutoCompletedConsole : ITextConsole
	{
		IAutoCompleteSource? AutoCompleteSource { get; set; }
	}
}