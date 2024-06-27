using System;
using System.Threading.Tasks;
using Core;
using TMPro;

namespace GameplaySystems.Chat
{
	public sealed class ChatBoxRequest
	{
		private readonly TaskCompletionSource<ChatBoxController> completionSource;
		private readonly Action onFulfilled;
		
		public ObjectId ChannelId { get; }

		public ChatBoxRequest(ObjectId channelId, TaskCompletionSource<ChatBoxController> completionSource, Action onFulfilled)
		{
			this.onFulfilled = onFulfilled;
			this.completionSource = completionSource;
			this.ChannelId = channelId;
		}

		public Task GiveControlAndWaitForRelease(TMP_InputField chatBox)
		{
			var releaseSource = new TaskCompletionSource<bool>();
			var controller = new ChatBoxController(releaseSource, chatBox);
			
			this.completionSource.SetResult(controller);

			onFulfilled?.Invoke();
			return releaseSource.Task;
		}
	}
}