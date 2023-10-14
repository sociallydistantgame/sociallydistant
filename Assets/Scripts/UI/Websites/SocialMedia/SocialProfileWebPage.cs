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
using UniRx;
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

		[SerializeField]
		private SocialPostListView listView = null!;

		private readonly ReactiveCollection<SocialPostModel> uiPosts = new ReactiveCollection<SocialPostModel>();

		private SocialMediaWebsite website;
		private IProfile? profile = null;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SocialProfileWebPage));
			this.MustGetComponentInParent(out website);
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			uiPosts.ObserveCountChanged(true).Subscribe(OnUiPostsChanged);
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

			SetupInitialFeed(isBlockedOrPrivate);
		}

		private void SetupInitialFeed(bool isBlockedOrPrivate)
		{
			this.uiPosts.Clear();

			if (isBlockedOrPrivate)
				return;

			if (socialService.Value == null)
				return;

			if (this.profile == null)
				return;

			foreach (IUserMessage post in socialService.Value.GetSocialPosts(this.profile))
			{
				uiPosts.Add(ConvertPost(post));
			}

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

			ListWidget? list = null;
			foreach (IProfile follower in socialService.Value.GetFollowers(profile))
			{
				if (list == null)
				{
					list = new ListWidget { AllowSelectNone = true };
					builder.AddWidget(list, followers);
				}
				
				builder.AddWidget(new ListItemWidget<IProfile>()
				{
					List = list,
					Data = follower,
					Title = $"{follower.ChatName}{Environment.NewLine}@{follower.SocialHandle}",
					Callback = ShowProfile,
					Selected = false
				}, followers);

				followerCount++;
			}

			list = null;
			foreach (IProfile follower in socialService.Value.GetFollowing(profile))
			{
				if (list == null)
				{
					list = new ListWidget { AllowSelectNone = true };
					builder.AddWidget(list, following);
				}
				
				builder.AddWidget(new ListItemWidget<IProfile>()
				{
					List = list,
					Data = follower,
					Title = $"{follower.ChatName}{Environment.NewLine}@{follower.SocialHandle}",
					Callback = ShowProfile,
					Selected = false
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

		private SocialPostModel ConvertPost(IUserMessage message)
		{
			return new SocialPostModel
			{
				Document = message.GetDocumentData(),
				Name = message.Author.ChatName,
				Handle = message.Author.SocialHandle
			};
		}

		private void OnUiPostsChanged(int count)
		{
			this.listView.SetItems(uiPosts);
		}
	}
}