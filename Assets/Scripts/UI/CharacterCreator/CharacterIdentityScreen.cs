using System;
using System.Collections.Generic;
using System.Linq;
using AcidicGui.Widgets;
using Core;
using UI.Social;
using UI.SystemSettings;
using UI.Widgets;
using UI.Widgets.Settings;
using UnityEngine;
using UnityEngine.Analytics;
using UnityExtensions;

namespace UI.CharacterCreator
{
	public class CharacterIdentityScreen : CharacterCreatorView
	{
		[SerializeField]
		private SocialProfileInfoView socialProfile = null!;
		
		[SerializeField]
		private WidgetList widgetList;

		private CharacterCreatorState state;

		/// <inheritdoc />
		public override bool CanGoForward =>
			!string.IsNullOrWhiteSpace(state.PlayerName);

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(CharacterIdentityScreen));
			base.Awake();
		}

		/// <inheritdoc />
		public override void SetData(CharacterCreatorState data)
		{
			this.state = data;

			this.RefreshProfile();
			this.RefreshWidgets();
		}

		private void RefreshProfile()
		{
			if (state == null)
				return;

			if (string.IsNullOrWhiteSpace(state.PlayerName))
			{
				socialProfile.FullName = "Your Name";
			}
			else
			{
				socialProfile.FullName = state.PlayerName;
			}

			socialProfile.Pronoun = state.ChosenGender;
		}

		private void RefreshWidgets()
		{
			var builder = new WidgetBuilder();

			builder.Begin();

			builder.AddSection("Personal Identity", out SectionWidget section)
				.AddWidget(new SettingsInputFieldWidget
				{
					Title = "Character Name",
					CurrentValue = state.PlayerName,
					Callback = value =>
					{
						state.PlayerName = value;
						RefreshProfile();
					}
				}).AddWidget(new SettingsDropdownWidget
				{
					Title = "Gender Identity",
					Choices = Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(x => SociallyDistantUtility.GetGenderDisplayString(x)).ToArray(),
					CurrentIndex = (int) state.ChosenGender,
					Callback = value =>
					{
						state.ChosenGender = (Gender) value;
						RefreshProfile();
					}
				});
			
			IList<IWidget> widgets = builder.Build();

			this.widgetList.SetItems(widgets);
		}
	}
}