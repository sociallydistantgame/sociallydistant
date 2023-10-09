#nullable enable

using System;
using System.Text;
using Core;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Analytics;
using UnityExtensions;

namespace UI.Social
{
	public class SocialProfileInfoView : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField]
		private Image coverImage = null!;

		[SerializeField]
		private Image avatarImage = null!;

		[SerializeField]
		private TextMeshProUGUI fullNameText = null!;

		[SerializeField]
		private TextMeshProUGUI usernameText = null!;

		[SerializeField]
		private TextMeshProUGUI bioText = null!;

		[SerializeField]
		private TextMeshProUGUI statsText = null!;
		
		[SerializeField]
		private TextMeshProUGUI pronounText = null!;

		private int followerCount;
		private int followingCount;
		private Gender pronoun;
		
		public string FullName
		{
			get => fullNameText.text;
			set => fullNameText.SetText(value);
		}

		public string Username
		{
			get => usernameText.text;
			set => usernameText.SetText(value);
		}

		public string Bio
		{
			get => bioText.text;
			set => bioText.SetText(value);
		}
		
		public Gender Pronoun
		{
			get => pronoun;
			set
			{
				pronoun = value;
				UpdatePronoun();
			}
		}

		public bool ShowStats
		{
			get => statsText.gameObject.activeSelf;
			set => statsText.gameObject.SetActive(value);
		}

		public bool ShowPronoun
		{
			get => pronounText.gameObject.activeSelf;
			set => pronounText.gameObject.SetActive(value);
		}
		
		public int FollowerCount
		{
			get => followerCount;
			set
			{
				followerCount = value;
				UpdateStats();
			}
		}

		public int FollowingCount
		{
			get => followingCount;
			set
			{
				followingCount = value;
				UpdateStats();
			}
		}
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SocialProfileInfoView));
			UpdateStats();
			UpdatePronoun();
		}

		private void UpdatePronoun()
		{
			pronounText.SetText(SociallyDistantUtility.GetGenderDisplayString(pronoun));
		}

		private void UpdateStats()
		{
			var sb = new StringBuilder();

			sb.Append("<b>");
			sb.Append(followerCount);
			sb.Append("</b> follower");
			if (followerCount != 1)
				sb.Append("s");

			sb.Append("  <b>");
			sb.Append(followingCount);
			sb.Append("</b> following");
			
			statsText.SetText(sb);
		}
	}
}