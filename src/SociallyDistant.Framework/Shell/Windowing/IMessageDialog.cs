#nullable enable

namespace SociallyDistant.Core.Shell.Windowing
{
	public interface IMessageDialog : ISystemWindow
	{
		string Message { get; set; }
		CommonColor Color { get; set; }
		MessageBoxType MessageType { get; set; }
		
		IList<MessageBoxButtonData> Buttons { get; }

		Action<MessageDialogResult>? DismissCallback { get; set; }
	}
}