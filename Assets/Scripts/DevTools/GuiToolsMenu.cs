#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using ContentManagement;
using Core;
using Core.WorldData.Data;
using GamePlatform;
using GameplaySystems.Missions;
using GameplaySystems.Social;
using Missions;
using Player;
using Social;
using UI.PlayerUI;
using UnityEngine;

namespace DevTools
{
	public class GuiToolsMenu : IDevMenu
	{
		private readonly PlayerInstanceHolder playerInstance;
		
		/// <inheritdoc />
		public string Name => "GUI tools";

		public GuiToolsMenu(PlayerInstanceHolder holder)
		{
			this.playerInstance = holder;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (playerInstance.Value.UiManager == null)
			{
				GUILayout.Label("Sorry, UiManager isn't available!");
				return;
			}
		}

		private void GuiTool(string name, DeveloperMenu devMenu, Action<DeveloperMenu, UiManager> action)
		{
			if (!GUILayout.Button(name))
				return;
			
			action?.Invoke(devMenu, playerInstance.Value.UiManager);
		}
	}

	public sealed class EmailMenu : IDevMenu
	{
		private readonly SocialServiceHolder social = null!;

		public EmailMenu(SocialServiceHolder social)
		{
			this.social = social;
		}

		/// <inheritdoc />
		public string Name => "Email Menu";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			ISocialService social = GameManager.Instance.SocialService;
			
			var world = WorldManager.Instance;
			
			if (GUILayout.Button("Send Message To Player"))
				devMenu.PushMenu(new SendMessageToProfile(world, social.PlayerProfile));
		}
	}

	public sealed class SendMessageToProfile : IDevMenu
	{
		private readonly WorldManager world;
		private readonly IProfile to;

		private string messageText;
		private WorldMailData mail = new();
		
		/// <inheritdoc />
		public string Name => "Send Mail To User";

		public SendMessageToProfile(WorldManager world, IProfile to)
		{
			this.world = world;
			this.to = to;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			GUILayout.Label($"To: {to.ChatName}");
			
			GUILayout.Label("Subject:");
			mail.Subject = GUILayout.TextField(mail.Subject);
			
			GUILayout.Label("Body (TMPro formatting supported)");
			messageText = GUILayout.TextArea(messageText);

			if (!GUILayout.Button("Send"))
				return;

			mail.InstanceId = world.GetNextObjectId();
			mail.To = to.ProfileId;
			mail.From = GetDeveloperMessagesId();
			mail.ThreadId = world.GetNextObjectId();

			var document = new List<DocumentElement>()
			{
				new DocumentElement
				{
					ElementType = DocumentElementType.Text,
					Data = messageText
				}
			};

			this.mail.Document = document;
			
			this.world.World.Emails.Add(mail);
			devMenu.PopMenu();
		}

		private ObjectId GetDeveloperMessagesId()
		{
			const string username = "acidiclight_dev_messages";
			WorldProfileData profile = world.World.Profiles.FirstOrDefault(x => x.ChatUsername == username);
			
			if (profile.ChatUsername != username)
			{
				profile.InstanceId = world.GetNextObjectId();
				profile.ChatUsername = username;
				profile.ChatName = "Developer Messages";
				world.World.Profiles.Add(profile);
			}

			return profile.InstanceId;
		}
	}

	public sealed class MissionDebug : IDevMenu
	{
		private readonly GameManager game;
		
		/// <inheritdoc />
		public string Name => "Mission Manager";

		public MissionDebug(GameManager game)
		{
			this.game = game;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			var world = WorldManager.Instance;
			var missions = MissionManager.Instance;

			if (missions == null)
			{
				GUILayout.Label("Mission Manager is unavailable.");
				return;
			}

			if (game.CurrentGameMode != GameMode.OnDesktop)
			{
				GUILayout.Label("Missions can not be debugged in the current game mode.");
				return;
			}
			
			GUILayout.Label("Current Mission Status");
			if (missions.CurrentMission == null)
			{
				GUILayout.Label("Free Roam (No mission active)");
			}
			else
			{
				GUILayout.Label($"{missions.CurrentMission.Id} ({missions.CurrentMission.Name})");

				GUILayout.Label($"Abandonable through UI: {missions.CAnAbandonMissions}");
				
				if (GUILayout.Button("Return to Free Roam (abandon mission forcibly)"))
				{
					missions.AbandonMission();
				}
			}

			if (GUILayout.Button("Start Mission"))
			{
				devMenu.PushMenu(new StartMissionDebug(missions, game.ContentManager));
				return;
			}
		}
	}

	public sealed class StartMissionDebug : IDevMenu
	{
		private readonly MissionManager missionManager;
		private readonly IContentManager contentManager;
		
		/// <inheritdoc />
		public string Name => "Start a Mission";

		public StartMissionDebug(MissionManager missionManager, IContentManager contentManager)
		{
			this.missionManager = missionManager;
			this.contentManager = contentManager;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			foreach (IMission mission in contentManager.GetContentOfType<IMission>())
			{
				if (GUILayout.Button($"{mission.Id} ({mission.Name})"))
				{
					missionManager.StartMission(mission);
					devMenu.PopMenu();
				}
			}
		}
	}
}