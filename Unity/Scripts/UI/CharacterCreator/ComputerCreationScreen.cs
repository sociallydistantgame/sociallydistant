using System.Collections.Generic;
using System.Text;
using AcidicGui.Widgets;
using Core;
using TMPro;
using UI.Widgets;
using UI.Widgets.Settings;
using UnityEngine;
using UnityExtensions;

namespace UI.CharacterCreator
{
	public class ComputerCreationScreen : CharacterCreatorView
	{
		[SerializeField]
		private WidgetList widgetList = null!;

		[SerializeField]
		private TextMeshProUGUI terminalText = null!;

		/// <inheritdoc />
		public override bool CanGoForward =>
			SociallyDistantUtility.IsPosixName(state.UserName) &&
			SociallyDistantUtility.IsPosixName(state.HostName);

		private readonly StringBuilder stringBuilder = new StringBuilder();
		private CharacterCreatorState state;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ComputerCreationScreen));
			base.Awake();
		}

		/// <inheritdoc />
		public override void SetData(CharacterCreatorState data)
		{
			this.state = data;
			RefreshTerminal();
			RefreshWidgets();
		}

		private void RefreshTerminal()
		{
			stringBuilder.Length = 0;

			if (string.IsNullOrWhiteSpace(state.UserName))
			{
				stringBuilder.Append("<color=#f71b1b>username</color>");
			}
			else
			{
				bool isUnix = SociallyDistantUtility.IsPosixName(state.UserName);

				if (!isUnix)
					stringBuilder.Append("<color=#f71b1b>");

				stringBuilder.Append(state.UserName);

				if (!isUnix)
					stringBuilder.Append("</color>");
			}

			stringBuilder.Append("@");
			
			if (string.IsNullOrWhiteSpace(state.HostName))
			{
				stringBuilder.Append("<color=#f71b1b>hostname</color>");
			}
			else
			{
				bool isUnix = SociallyDistantUtility.IsPosixName(state.HostName);

				if (!isUnix)
					stringBuilder.Append("<color=#f71b1b>");

				stringBuilder.Append(state.HostName);

				if (!isUnix)
					stringBuilder.Append("</color>");
			}

			stringBuilder.Append($":~$ whoami<mark=#ffffffff>_</mark>");
			
			this.terminalText.SetText(stringBuilder);
		}

		private void RefreshWidgets()
		{
			var builder = new WidgetBuilder();

			builder.Begin();

			builder.AddWidget(new SettingsFieldWidget
			{
				Title = "Your Username",
				Slot = new InputFieldWidget
				{
					Value = state.UserName,
					Callback = value =>
					{
						state.UserName = value;
						RefreshTerminal();
					}
				}
			}).AddWidget(new SettingsFieldWidget
			{
				Title = "System Hostname",
				Slot = new InputFieldWidget
				{
					Value = state.HostName,
					Callback = value =>
					{
						state.HostName = value;
						RefreshTerminal();
					}
				}
			});
			
			IList<IWidget> widgets = builder.Build();
			
			this.widgetList.SetItems(widgets);
		}
	}
}