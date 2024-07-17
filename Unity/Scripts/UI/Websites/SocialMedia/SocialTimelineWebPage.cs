#nullable enable
using System;
using AcidicGui.Widgets;
using GameplaySystems.Social;
using GameplaySystems.WebPages;
using Social;
using UI.Widgets;
using UniRx;
using UnityEngine;
using UnityExtensions;

namespace UI.Websites.SocialMedia
{
	public class SocialTimelineWebPage : WebPage
	{
		[Header("Dependencies")]
		
		private SocialServiceHolder socialService = null!;
		
		[Header("UI")]
		
		private SocialPostListView listView = null!;

		
		private WidgetList sidebar = null!;
        
		private readonly ReactiveCollection<SocialPostModel> uiPosts = new ReactiveCollection<SocialPostModel>();
		private SocialMediaWebsite website;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SocialTimelineWebPage));
			this.MustGetComponentInParent(out website);
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			uiPosts.ObserveCountChanged(true).Subscribe(OnPostCountChanged);
		}

		/// <inheritdoc />
		protected override void OnShow(Action? callback = null)
		{
			base.OnShow(callback);

			RefreshKnownUsers();
			RefreshTimeline();
		}

		private void RefreshTimeline()
		{
			uiPosts.Clear();

			if (socialService.Value == null)
				return;

			foreach (IUserMessage post in socialService.Value.GetTimeline(socialService.Value.PlayerProfile))
			{
				uiPosts.Add(ConvertPost(post));
			}
		}

		private void OnPostCountChanged(int count)
		{
			listView.SetItems(uiPosts);
		}

		private void RefreshKnownUsers()
		{
			var builder = new WidgetBuilder();
			builder.Begin();

			builder.AddSection("People you may know", out SectionWidget people);
			
			var knownUserCount = 0;

			// People who the player follows are prioritized.
			if (socialService.Value != null)
			{
				foreach (IProfile profile in socialService.Value.GetFollowing(socialService.Value.PlayerProfile))
				{
					builder.AddWidget(new ListItemWidget<IProfile>
					{
						Data = profile,
						Title = $"<b>{profile.ChatName}</b>{Environment.NewLine}@{profile.SocialHandle}",
						Callback = (data) =>
						{
							website.SetHistoryState($"/profile/{data.SocialHandle}", () =>
							{
								website.ShowProfile(data);
							});
						}
					}, people);

					knownUserCount++;
				}
			}
			
			if (knownUserCount == 0)
			{
				builder.AddWidget(new LabelWidget
				{
					Text = "You don't know anyone yet. People you discover and people you follow will appear here."
				}, people);
			}
			
			sidebar.SetItems(builder.Build());
		} 
	}
}