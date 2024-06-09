#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContentManagement;
using Core;
using Modules;
using Social;

namespace Missions
{
	public enum MissionType
	{
		Main,
		Side,
		RandomEncounter
	}

	public enum DangerLevel
	{
		None,
		Minor,
		Moderate,
		Dangerous
	}

	public enum MissionStartCondition
	{
		Auto,
		Scripted,
		Email,
	}

	public interface IMission : IGameContent
	{
		string Id { get; }
		string Name { get; }
		DangerLevel DangerLevel { get; }
		MissionType Type { get; }
		MissionStartCondition StartCondition { get; }
		string GiverId { get; }

		bool IsAvailable(IWorld world);
		bool IsCompleted(IWorld world);

		Task<string> GetBriefingText(IProfile playerProfile);

		Task StartMission(IMissionController missionController, CancellationToken cancellationToken);
	}

	public interface IMissionController
	{
		IGameContext Game { get; }
		IWorldManager WorldManager { get; }
		bool CanAbandonMission { get; }
		IReadOnlyList<IObjective> CurrentObjectives { get; }
		
		void DisableAbandonment();
		void EnableAbandonment();

		IObjectiveHandle CreateObjective(string name, string description, bool isChallenge);
		IDisposable ObserveObjectivesChanged(Action<IReadOnlyList<IObjective>> callback);
	}

	public interface IObjective
	{
		string Name { get; }
		string Description { get; }
		bool IsOptionalChallenge { get; }
		bool IsCompleted { get; }
		bool IsFailed { get; }
		string? FailMessage { get; }
		string? Hint { get; }
	}
	
	public interface IObjectiveHandle
	{
		string Name { get; set; }
		string Description { get; set; }
		bool IsOptionalChallenge { get; set; }
		bool IsFAiled { get; }
		string? Hint { get; set; }
		
		void MarkCompleted();
		void MarkFailed(string reason);
	}

	public enum ObjectiveType
	{
		Scripted
	}
}