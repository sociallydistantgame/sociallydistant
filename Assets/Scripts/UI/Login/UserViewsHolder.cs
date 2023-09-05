#nullable enable

using Com.TheFallenGames.OSA.Core;
using UI.Widgets;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Login
{
	public class UserViewsHolder : BaseItemViewsHolder
	{
		private Button button;
		private TwoLineButtonWithIcon view;

		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponent(out button);
			root.MustGetComponent(out view);
			base.CollectViews();
		}

		public void UpdateView(UserListItemModel model)
		{
			
		}
	}
}