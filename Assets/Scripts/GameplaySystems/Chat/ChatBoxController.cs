using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace GameplaySystems.Chat
{
	public sealed class ChatBoxController
	{
		private readonly TMP_InputField inputField;
		private readonly TaskCompletionSource<bool> releaseTask;

		public ChatBoxController(TaskCompletionSource<bool> releaseTask, TMP_InputField inputField)
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