#nullable enable
namespace SociallyDistant.Core.Shell.Windowing
{
	public struct MessageBoxButtonData
	{
		public string Text;
		public Action? ClickHandler;
		public MessageDialogResult Result;

		public MessageBoxButtonData(string text, MessageDialogResult result)
		{
			this.Text = text;
			this.Result = result;
		}
		
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