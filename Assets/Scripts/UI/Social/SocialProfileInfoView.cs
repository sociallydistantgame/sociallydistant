#nullable enable

using System;
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
		private TextMeshProUGUI pronounText = null!;

		private Gender pronoun;
		
		public string FullName
		{
			get => fullNameText.text;
			set => fullNameText.SetText(value);
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
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SocialProfileInfoView));
			UpdatePronoun();
		}

		private void UpdatePronoun()
		{
			pronounText.SetText(SociallyDistantUtility.GetGenderDisplayString(pronoun));
		}
	}
}