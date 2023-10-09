#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AcidicGui.Widgets;
using GameplaySystems.Social;
using GameplaySystems.WebPages;
using Social;
using UI.Social;
using UI.Widgets;
using UnityEngine;
using UnityExtensions;

namespace UI.Websites.SocialMedia
{
	public class SocialProfileWebPage : WebPage
	{
		[Header("Dependencies")]
		[SerializeField]
		private SocialServiceHolder socialService = null!;
		
		[Header("UI")]
		[SerializeField]
		private SocialProfileInfoView profileInfo = null!;

		[SerializeField]
		private WidgetList sidebar = null!;
		
		private IProfile? profile = null;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SocialProfileWebPage));
			base.Awake();
		}

		public void ShowProfile(IProfile profile)
		{
			this.profileInfo.FullName = profile.ChatName;
			this.profileInfo.Username = $"@{profile.SocialHandle}";
			this.profileInfo.Pronoun = profile.Gender;
			this.profileInfo.Bio = profile.Bio;

			this.profile = profile;

			var isPrivate = false;
			var isBlocked = false;

			if (socialService.Value != null)
			{
				profileInfo.FollowerCount = socialService.Value.GetFollowers(profile).Count();
				profileInfo.FollowingCount = socialService.Value.GetFollowing(profile).Count();
                
				IProfile player = socialService.Value.PlayerProfile;

				isBlocked = socialService.Value.GetBlockedProfiles(profile)
					.Contains(player);

				isPrivate = isBlocked
				            || (profile != player
				                && profile.IsPrivate
				                && !socialService.Value.GetFollowing(profile).Contains(player));
			}
			else
			{
				profileInfo.FollowingCount = 0;
				profileInfo.FollowerCount = 0;
			}
			
			bool isBlockedOrPrivate = isPrivate || isBlocked;

			profileInfo.ShowStats = !isBlockedOrPrivate;
			profileInfo.ShowPronoun = !isBlockedOrPrivate;
			
			sidebar.SetItems(BuildRelationships(isBlockedOrPrivate));
		}

		private IList<IWidget> BuildRelationships(bool isBlockedOrPrivate)
		{
			var builder = new WidgetBuilder();

			builder.Begin();

			if (profile == null)
				return builder.Build();
			
			if (socialService.Value == null)
				return builder.Build();
            
			if (isBlockedOrPrivate)
				return builder.Build();

			builder.AddSection("Followers", out SectionWidget followers);
			builder.AddSection("Following", out SectionWidget following);

			var followerCount = 0;
			var followingCount = 0;
			
			foreach (IProfile follower in socialService.Value.GetFollowers(profile))
			{
				builder.AddWidget(new ListItemWidget<IProfile>()
				{
					Data = follower,
					Title = $"{profile.ChatName}{Environment.NewLine}@{profile.SocialHandle}",
					Callback = ShowProfile
				}, followers);

				followerCount++;
			}
			
			foreach (IProfile follower in socialService.Value.GetFollowing(profile))
			{
				builder.AddWidget(new ListItemWidget<IProfile>()
				{
					Data = follower,
					Title = $"{profile.ChatName}{Environment.NewLine}@{profile.SocialHandle}",
					Callback = ShowProfile
				}, following);

				followingCount++;
			}

			if (followerCount == 0)
				builder.AddWidget(new LabelWidget
				{
					Text = "This user doesn't have any followers."
				}, followers);
			
			if (followingCount == 0)
				builder.AddWidget(new LabelWidget
				{
					Text = "This user isn't following anyone."
				}, following);
			
			return builder.Build();
		}
	}
}