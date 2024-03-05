#nullable enable

using Com.TheFallenGames.OSA.Core;
using UI.Widgets;
using UnityEngine.UI;
using UnityExtensions;
using Utility;
using System;
using GamePlatform;
using Shell;
using UI.Shell.InfoPanel;

namespace UI.Login
{
	public class UserViewsHolder : AutoSizedItemsViewsHolder
	{
		private Button button;
		private TwoLineButtonWithIcon view;
		private IGameData? gameData;

		public Action<IGameData?>? Callback;
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out button);
			root.MustGetComponentInChildren(out view);
			
			button.onClick.AddListener(OnClick);
			
			base.CollectViews();
		}

		public void UpdateView(UserListItemModel model)
		{
			view.Icon = MaterialIcons.AccountCircle;
			view.FirstLine = model.Name;
			view.SecondLine = model.Comments;

			gameData = model.GameData;
		}

		private void OnClick()
		{
			Callback?.Invoke(gameData);
		}
	}
}