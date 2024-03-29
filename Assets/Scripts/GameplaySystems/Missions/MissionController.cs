#nullable enable
using System;
using System.Collections.Generic;
using Core;
using GamePlatform;
using Missions;
using Modules;
using UniRx;
using UnityEngine;

namespace GameplaySystems.Missions
{
	public sealed class MissionController : IMissionController
	{
		private readonly List<IObjective> objectives = new();
		private readonly Subject<IReadOnlyList<IObjective>> objectiveUpdateSubject = new();
		private readonly MissionManager missionManager;
		private readonly WorldManagerHolder worldManager;
		private readonly GameManagerHolder gameManagerHolder;

		/// <inheritdoc />
		public IGameContext Game => gameManagerHolder.Value!;
        
		/// <inheritdoc />
		public IWorldManager WorldManager => worldManager.Value!; // if this is null, we've already had a catastrophe anyway.

		/// <inheritdoc />
		public bool CanAbandonMission { get; private set; } = true;

		/// <inheritdoc />
		public IReadOnlyList<IObjective> CurrentObjectives => objectives;

		public MissionController(MissionManager missionManager, WorldManagerHolder worldManager, GameManagerHolder gameManagerHolder)
		{
			this.missionManager = missionManager;
			this.worldManager = worldManager;
			this.gameManagerHolder = gameManagerHolder;
		}

		/// <inheritdoc />
		public void DisableAbandonment()
		{
			Debug.Log("Mission abandonment has been disabled.");
			CanAbandonMission = false;
		}

		/// <inheritdoc />
		public void EnableAbandonment()
		{
			Debug.Log("Mission abandonment has been enabled.");
			CanAbandonMission = true;
		}

		/// <inheritdoc />
		public IObjectiveHandle CreateObjective(string name, string description, bool isChallenge)
		{
			var controller = new ObjectiveController(this.SignalObjectiveUpdate);

			controller.Name = name;
			controller.Description = description;
			controller.IsOptional = isChallenge;
			
			var objective = new Objective(controller);

			this.objectives.Add(objective);

			return new ObjectiveHandle(controller);
		}

		/// <inheritdoc />
		public IDisposable ObserveObjectivesChanged(Action<IReadOnlyList<IObjective>> callback)
		{
			return objectiveUpdateSubject.Subscribe(callback);
		}

		private void SignalObjectiveUpdate()
		{
			this.objectiveUpdateSubject.OnNext(this.CurrentObjectives);
		}
	}
}