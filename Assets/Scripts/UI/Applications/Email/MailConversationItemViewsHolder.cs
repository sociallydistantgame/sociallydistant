#nullable enable
using System;
using Social;
using UI.Shell.InfoPanel;
using UnityExtensions;

namespace UI.Applications.Email
{
	public sealed class MailConversationItemViewsHolder : AutoSizedItemsViewsHolder
	{
		private ConversationItemView view;

		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out view);
			base.CollectViews();
		}

		public void UpdateViews(IMailMessage message)
		{
			view.UpdateView(message);
		}
	}
}