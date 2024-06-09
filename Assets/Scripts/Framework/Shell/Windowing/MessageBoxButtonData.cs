#nullable enable
namespace Shell.Windowing
{
	public struct MessageBoxButtonData
	{
		public string Text;
		public MessageDialogResult Result;
		
		public static implicit operator MessageBoxButtonData(string text)
		{
			return new MessageBoxButtonData
			{
				Result = MessageDialogResult.Ok,
				Text = text
			};
		}
	}
}