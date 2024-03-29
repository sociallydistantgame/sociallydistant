#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Scripting;
using Core.WorldData;
using Core.WorldData.Data;
using GamePlatform;
using GameplaySystems.Social;
using Missions;
using Modules;
using OS.Devices;
using Social;
using UnityEngine;
using UnityEngine.Analytics;
using UnityExtensions;

namespace GameplaySystems.Missions
{
	public sealed class MissionManager : MonoBehaviour
	{
		[SerializeField]
		private GameManagerHolder gameManager = null!;

		[SerializeField]
		private WorldManagerHolder worldManager = null!;

		[SerializeField]
		private SocialServiceHolder socialService = null!;

		private StartMissionCommand startMissionCommand;
		private MissionMailerHook missionMailerHook;
		private CancellationTokenSource? missionAnandonSource;
		private IMission? currentMission;
		private Task? currentMissionTask;
		private IMissionController missionController;

		public IMission? CurrentMission => currentMission;

		public bool CAnAbandonMissions => CurrentMission != null && missionController.CanAbandonMission;
		public bool CanStartMissions => worldManager.Value != null
		                                && gameManager.Value != null
		                                && gameManager.Value.CurrentGameMode == GameMode.OnDesktop
		                                && (this.currentMission == null || missionController.CanAbandonMission);
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(MissionManager));

			this.startMissionCommand = new StartMissionCommand(this);
			this.missionMailerHook = new MissionMailerHook(this);
			this.missionController = new MissionController(this, this.worldManager, this.gameManager);
		}

		private void OnEnable()
		{
			if (gameManager.Value != null)
			{
				gameManager.Value.UriManager.RegisterSchema("mission", new MissionUriSchemeHandler(this));
			}
		}

		private void OnDisable()
		{
			if (gameManager.Value != null)
			{
				gameManager.Value.UriManager.UnregisterSchema("mission");
			}
		}

		private void Start()
		{
			if (gameManager.Value == null)
				return;
			
			gameManager.Value.ScriptSystem.RegisterGlobalCommand("start_mission", startMissionCommand);
			gameManager.Value.ScriptSystem.RegisterHookListener(CommonScriptHooks.AfterWorldStateUpdate, missionMailerHook);
		}

		private void OnDestroy()
		{
			if (gameManager.Value == null)
				return;

			gameManager.Value.ScriptSystem.UnregisterHookListener(CommonScriptHooks.AfterWorldStateUpdate, missionMailerHook);
			gameManager.Value.ScriptSystem.UnregisterGlobalCommand("start_mission");
		}

		private void Update()
		{
			if (currentMissionTask != null)
			{
				if (currentMissionTask.Exception != null)
				{
					Debug.LogException(currentMissionTask.Exception);
					AbandonMission();
				}
				else if (currentMissionTask.IsCompleted)
				{
					CompleteMission();
				}
			}
		}

		private async void CompleteMission()
		{
			if (this.currentMission == null)
				return;

			if (worldManager.Value != null)
			{
				ProtectedWorldState protectedState = worldManager.Value.World.ProtectedWorldData.Value;
				var missionList = new List<string>();
				missionList.AddRange(protectedState.CompletedMissions);
				missionList.Add(this.currentMission.Id);

				protectedState.CompletedMissions = missionList;

				worldManager.Value.World.ProtectedWorldData.Value = protectedState;
			}

			this.currentMission = null;
			this.currentMissionTask = null;
			this.missionAnandonSource = null;

			if (this.gameManager.Value != null)
				await gameManager.Value.SaveCurrentGame(false);
		}
		
		public void AbandonMission()
		{
			if (this.currentMission == null)
				return;

			this.missionAnandonSource?.Cancel();
			this.currentMission = null;
			this.currentMissionTask = null;
		}
		
		public bool StartMission(IMission mission)
		{
			if (worldManager.Value == null)
				return false;

			if (mission.IsCompleted(worldManager.Value.World))
				return false;
			
			if (!mission.IsAvailable(worldManager.Value.World))
				return false;
			
			if (this.currentMission != null)
			{
				if (!missionController.CanAbandonMission)
					return false;
				
				this.AbandonMission();
			}

			this.currentMission = mission;
			this.missionAnandonSource = new CancellationTokenSource();
			this.currentMissionTask = this.currentMission.StartMission(this.missionController, this.missionAnandonSource.Token);
			return true;
		}
		
		public IMission? GetMissionById(string missionId)
		{
			if (gameManager.Value == null)
				return null;

			return gameManager.Value.ContentManager.GetContentOfType<IMission>()
				.FirstOrDefault(x => x.Id == missionId);
		}

		private IProfile ResolveGiver(string giverId)
		{
			if (socialService.Value == null)
				throw new InvalidOperationException("SocialService is null");
			
			if (worldManager.Value == null)
				throw new InvalidOperationException("WorldManager is null");

			WorldProfileData profileData = worldManager.Value.World.Profiles.FirstOrDefault(x => x.NarrativeId == giverId);
			if (profileData.NarrativeId != giverId)
			{
				profileData.InstanceId = worldManager.Value.GetNextObjectId();
				profileData.NarrativeId = giverId;
				profileData.ChatUsername = giverId;
				profileData.ChatName = giverId;
				profileData.Gender = Gender.Unknown;
				worldManager.Value.World.Profiles.Add(profileData);
			}

			return socialService.Value.GetProfileById(profileData.InstanceId);
		}
		
		private async Task MailMission(IMission mission)
		{
			if (socialService.Value == null)
				return;

			if (worldManager.Value == null)
				return;
			
			IProfile giver = ResolveGiver(mission.GiverId);
			IProfile player = socialService.Value.PlayerProfile;
			
			switch (mission.StartCondition)
			{
				case MissionStartCondition.Email:
				{
					// Find an existing briefing email that isn't marked as completed but has the narrative ID of the mission.
					// If we find it, then we'll update it with the mission's new briefing info. If we don't find
					// a matching mail object then we'll send a new one to the player.
					var isNewMail = false;
					WorldMailData mail = worldManager.Value.World.Emails.FirstOrDefault(x => x.NarrativeId == mission.Id
					                                                                         && x.TypeFlags.HasFlag(MailTypeFlags.Briefing));

					if (mail.NarrativeId != mission.Id)
					{
						mail.InstanceId = worldManager.Value.GetNextObjectId();
						mail.ThreadId = worldManager.Value.GetNextObjectId();
						mail.NarrativeId = mission.Id;
						mail.TypeFlags = MailTypeFlags.Briefing;
						isNewMail = true;
					}

					if (mission.IsCompleted(worldManager.Value.World))
					{
						mail.TypeFlags |= MailTypeFlags.CompletedMission;
					}
					else
					{
						mail.TypeFlags &= ~MailTypeFlags.CompletedMission;
					}
					
					mail.From = giver.ProfileId;
					mail.To = player.ProfileId;
					mail.Subject = mission.Name;

					string briefingText = await mission.GetBriefingText(player);

					mail.Document = briefingText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
						.Select(x => new DocumentElement
						{
							ElementType = DocumentElementType.Text,
							Data = x
						}).ToList();
					
					if (isNewMail)
						worldManager.Value.World.Emails.Add(mail);
					else
						worldManager.Value.World.Emails.Modify(mail);
					
					break;
				}
			}
		}
		
		private async Task MailMissions()
		{
			if (gameManager.Value == null)
				return;

			if (worldManager.Value == null)
				return;
			
			foreach (IMission mission in gameManager.Value.ContentManager.GetContentOfType<IMission>())
			{
				if (!mission.IsAvailable(worldManager.Value.World))
					continue;


				await MailMission(mission);
			}
		}
		
		private sealed class StartMissionCommand : IScriptCommand
		{
			private readonly MissionManager missionManager;

			public StartMissionCommand(MissionManager missionManager)
			{
				this.missionManager = missionManager;
			}
			
			/// <inheritdoc />
			public Task ExecuteAsync(IScriptExecutionContext context, ITextConsole console, string name, string[] args)
			{
				if (args.Length != 1)
					throw new InvalidOperationException($"{name}: usage: {name} <missionId>");

				string missionId = args[0];
				IMission? mission = missionManager.GetMissionById(missionId);

				if (mission == null)
					throw new InvalidOperationException($"Mission {missionId} does not exist.");

				missionManager.StartMission(mission);
				return Task.CompletedTask;
			}
		}

		private sealed class MissionMailerHook : IHookListener
		{
			private readonly MissionManager missionManager;

			public MissionMailerHook(MissionManager missionManager)
			{
				this.missionManager = missionManager;
			}
			
			/// <inheritdoc />
			public async Task ReceiveHookAsync(IGameContext game)
			{
				await missionManager.MailMissions();
			}
		}

		private sealed class MissionUriSchemeHandler : IUriSchemeHandler
		{
			private readonly MissionManager missionManager;

			public MissionUriSchemeHandler(MissionManager missionManager)
			{
				this.missionManager = missionManager;
			}
			
			/// <inheritdoc />
			public void HandleUri(Uri uri)
			{
				if (uri.Scheme != "mission")
					return;

				switch (uri.Host)
				{
					case "start":
					{
						string missionId = uri.AbsolutePath;

						IMission? mission = missionManager.GetMissionById(missionId);
						if (mission == null)
							return;

						missionManager.StartMission(mission);
						break;
					}
					case "abandon":
					{
						if (missionManager.CurrentMission == null)
							return;

						if (!missionManager.CAnAbandonMissions)
							return;

						missionManager.AbandonMission();
						break;
					}
				}
			}
		}
	}
}