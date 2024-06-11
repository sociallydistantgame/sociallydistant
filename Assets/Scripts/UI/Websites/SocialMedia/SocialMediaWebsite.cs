#nullable enable

using System;
using System.Linq;
using GamePlatform;
using GameplaySystems.Social;
using GameplaySystems.WebPages;
using JetBrains.Annotations;
using Social;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Websites.SocialMedia
{
	public class SocialMediaWebsite : WebSite
	{
		[Header("UI")]
		[SerializeField]
		private SocialTimelineWebPage timelineWebPage = null!;
		
		[SerializeField]
		private SocialProfileWebPage profileView = null!;

		[SerializeField]
		private Toggle playerProfileToggle = null!;
		
		[SerializeField]
		private Toggle otherPagesToggle = null!;

		private ISocialService socialService;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			socialService = GameManager.Instance.SocialService;
			
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(SocialMediaWebsite));
		}

		private void Start()
		{
			playerProfileToggle.onValueChanged.AddListener(OnPlayerProfileToggle);
		}

		/// <inheritdoc />
		protected override void GoToIndex()
		{
			NavigateTo(timelineWebPage);
		}

		[WebPage("profile", ":username")]
		[UsedImplicitly]
		private void ShowProfile(string username)
		{
			IProfile? user = socialService.Profiles
				.FirstOrDefault(x => x.SocialHandle == username);

			if (user == null)
				return;

			ShowProfile(user);
		}

		public void ShowProfile(IProfile profile)
		{
			if (profile == socialService.PlayerProfile)
			{
				playerProfileToggle.SetIsOnWithoutNotify(true);
			}
			else
			{
				otherPagesToggle.SetIsOnWithoutNotify(true);
			}
				
			
			NavigateTo(profileView, () =>
			{
				profileView.ShowProfile(profile);
			});
		}

		private void OnPlayerProfileToggle(bool value)
		{
			if (!value)
				return;
			
			ShowProfile(socialService.PlayerProfile);
		}
	}
}