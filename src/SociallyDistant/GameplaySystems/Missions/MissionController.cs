#nullable enable
using System.Diagnostics;
using System.Reactive.Subjects;
using Serilog;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Missions;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class MissionController : IMissionController
	{
		private readonly List<IObjective> objectives = new();
		private readonly Subject<IReadOnlyList<IObjective>> objectiveUpdateSubject = new();
		private readonly MissionManager missionManager;
		private readonly IWorldManager worldManager;
		private readonly SociallyDistantGame gameManagerHolder;

		/// <inheritdoc />
		public IGameContext Game => gameManagerHolder!;

		/// <inheritdoc />
		public IWorldManager WorldManager => worldManager;

		/// <inheritdoc />
		public bool CanAbandonMission { get; private set; } = true;

		/// <inheritdoc />
		public IReadOnlyList<IObjective> CurrentObjectives => objectives;

		internal MissionController(MissionManager missionManager, IWorldManager worldManager, SociallyDistantGame gameManagerHolder)
		{
			this.missionManager = missionManager;
			this.worldManager = worldManager;
			this.gameManagerHolder = gameManagerHolder;
		}

		/// <inheritdoc />
		public void DisableAbandonment()
		{
			Log.Information("Mission abandonment has been disabled.");
			CanAbandonMission = false;
		}

		/// <inheritdoc />
		public void EnableAbandonment()
		{
			Log.Information("Mission abandonment has been enabled.");
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