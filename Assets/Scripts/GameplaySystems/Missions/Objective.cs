using Missions;

namespace GameplaySystems.Missions
{
	public sealed class Objective : IObjective
	{
		private readonly ObjectiveController objectiveController;

		/// <inheritdoc />
		public string Name => objectiveController.Name;

		/// <inheritdoc />
		public string Description => objectiveController.Description;

		/// <inheritdoc />
		public string? Hint => objectiveController.Hint;
        
		/// <inheritdoc />
		public bool IsOptionalChallenge => objectiveController.IsOptional;

		/// <inheritdoc />
		public bool IsCompleted => objectiveController.IsCompleted;

		/// <inheritdoc />
		public bool IsFailed => objectiveController.IsFailed;

		/// <inheritdoc />
		public string? FailMessage => objectiveController.FailReason;

		public Objective(ObjectiveController controller)
		{
			this.objectiveController = controller;
		}
	}
}