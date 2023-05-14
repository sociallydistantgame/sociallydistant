namespace OS.Devices
{
	public interface IAutoCompletedConsole : ITextConsole
	{
		IAutoCompleteSource? AutoCompleteSource { get; set; }
	}
}