#nullable enable
using AcidicGui.Widgets;
using Core.WorldData.Data;
using GameplaySystems.Missions;
using Missions;
using Shell;
using Shell.InfoPanel;
using Social;
using UI.Widgets;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityExtensions;

namespace UI.Applications.Email
{
	public sealed class ConversationItemView : MonoBehaviour
	{
		[SerializeField]
		private StaticWidgetList widgetList = null!;
		
		private MissionManager? missionManager = null!;
		private IMailMessage? message = null;
		
		private void Awake()
		{
			missionManager = MissionManager.Instance;
			this.AssertAllFieldsAreSerialized(typeof(ConversationItemView));
		}

		public void UpdateView(IMailMessage message)
		{
			this.message = message;
			var widgetBuilder = new WidgetBuilder();

			widgetBuilder.Begin();

			widgetBuilder.AddLabel($"<b>From:</b> {message.From.ChatName}");
			widgetBuilder.AddLabel($"<b>To:</b> {message.To.ChatName}");

			foreach (DocumentElement element in message.Body)
			{
				BuildElement(widgetBuilder, element);
			}

			if (message.MessageType.HasFlag(MailTypeFlags.Briefing) && missionManager != null)
			{
				IMission? mission = missionManager.GetMissionById(message.NarrativeId);
				if (mission != null)
				{
					AddMissionEmbed(widgetBuilder, mission, message.MessageType.HasFlag(MailTypeFlags.CompletedMission));
				}
			}
			
			this.widgetList.UpdateWidgetList(widgetBuilder.Build());
		}

		private void AddMissionEmbed(WidgetBuilder builder, IMission mission, bool completed)
		{
			if (missionManager == null)
				return;

			EmbedBuilder embedBuilder = new EmbedBuilder()
				.WithTitle("Mission")
				.WithContent(mission.Name);
			
			if (completed)
			{
				embedBuilder = embedBuilder.WithColor(CommonColor.Green)
					.WithTitle("Mission - Completed");
			}
			else
			{
				embedBuilder = embedBuilder.WithColor(CommonColor.Yellow);

				if (this.missionManager.CurrentMission == mission)
					embedBuilder = embedBuilder.WithTitle("Mission - In Progress");

				if (this.missionManager.CanStartMissions && this.missionManager.CurrentMission != mission)
					embedBuilder = embedBuilder.WithAction("Start", () => StartMission(mission));

				if (this.missionManager.CurrentMission == mission && this.missionManager.CAnAbandonMissions)
					embedBuilder = embedBuilder.WithAction("Abandon mission", AbandonMission);
			}
			
			builder.AddWidget(new RichEmbedWidget
			{
				EmbedData = embedBuilder.Build()
			});
		}

		private void AbandonMission()
		{
			if (this.message == null)
				return;
			
			if (this.missionManager == null)
				return;

			if (!this.missionManager.CAnAbandonMissions)
				return;

			this.missionManager.AbandonMission();
			this.UpdateView(this.message);
		}
		
		private void StartMission(IMission mission)
		{
			if (this.message == null)
				return;
			
			if (missionManager == null)
				return;

			if (!missionManager.StartMission(mission))
				return;

			this.UpdateView(this.message);
		}
		
		private void BuildElement(WidgetBuilder builder, DocumentElement element)
		{
			switch (element.ElementType)
			{
				case DocumentElementType.Text:
					builder.AddLabel(element.Data);
					break;
			}
		}
	}
}