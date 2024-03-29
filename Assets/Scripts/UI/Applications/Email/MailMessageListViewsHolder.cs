#nullable enable
using Social;
using UI.Shell.InfoPanel;
using UnityExtensions;
using System;

namespace UI.Applications.Email
{
	public sealed class MailMessageListViewsHolder : AutoSizedItemsViewsHolder
	{
		private MailMessageView view;

		public Action<IMailMessage>? Callback { get; set; }

		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out view);
			base.CollectViews();
		}

		public void UpdateViews(IMailMessage message)
		{
			view.Callback = Callback;
			view.UpdateMessage(message);
		}
	}
}