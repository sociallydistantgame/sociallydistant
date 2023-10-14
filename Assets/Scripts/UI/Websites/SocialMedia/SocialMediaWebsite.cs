#nullable enable

using System;
using System.Linq;
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
		[Header("Dependencies")]
		[SerializeField]
		private SocialServiceHolder socialService = null!;

		[Header("UI")]
		[SerializeField]
		private SocialTimelineWebPage timelineWebPage = null!;
		
		[SerializeField]
		private SocialProfileWebPage profileView = null!;

		[SerializeField]
		private Toggle playerProfileToggle = null!;
		
		[SerializeField]
		private Toggle otherPagesToggle = null!;
		
		
		/// <inheritdoc />
		protected override void Awake()
		{
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
			if (socialService.Value == null)
				return;

			IProfile? user = socialService.Value.Profiles
				.FirstOrDefault(x => x.SocialHandle == username);

			if (user == null)
				return;

			ShowProfile(user);
		}

		public void ShowProfile(IProfile profile)
		{
			if (socialService.Value != null && profile == socialService.Value.PlayerProfile)
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

			if (socialService.Value == null)
				return;

			ShowProfile(socialService.Value.PlayerProfile);
		}
	}
}