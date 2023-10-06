using System;
using TMPro;
using UI.Shell.InfoPanel;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public class ServerMemberViewsHolder : AutoSizedItemsViewsHolder
	{
		private TextMeshProUGUI text;
		private RawImage image;
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			base.CollectViews();
			root.MustGetComponentInChildren(out image);
			root.MustGetComponentInChildren(out text);
		}

		public void UpdateMember(ServerMember memberModel)
		{
			image.texture = memberModel.Avatar;

			text.SetText($"<b>{memberModel.DisplayName}</b>{Environment.NewLine}@{memberModel.Username}");
		}
	}
}