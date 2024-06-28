using System.Text;

namespace SociallyDistant.GameplaySystems.Chat
{
	public sealed class ChatBoxController
	{
		private readonly IChatMessageField inputField;
		private readonly TaskCompletionSource<bool> releaseTask;

		public ChatBoxController(TaskCompletionSource<bool> releaseTask, IChatMessageField inputField)
		{
			this.releaseTask = releaseTask;
			this.inputField = inputField;
		}

		public void ReleaseControl()
		{
			releaseTask.SetResult(true);
		}

		public void SetTExt(StringBuilder stringBuilder)
		{
			inputField.SetTextWithoutNotify(stringBuilder.ToString());
		}
	}
}