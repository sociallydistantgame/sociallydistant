#nullable enable
using UI.Shell.InfoPanel;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public class ChatMessageViewsHolder : AutoSizedItemsViewsHolder
	{
		private ChatMessageView view;

		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out view);
			base.CollectViews();
		}

		public void UpdateView(ChatMessageModel model)
		{
			view.UpdateMessage(model);
		}

		/// <inheritdoc />
		public ChatMessageViewsHolder(int itemIndex) : base(itemIndex)
		{ }
	}
}