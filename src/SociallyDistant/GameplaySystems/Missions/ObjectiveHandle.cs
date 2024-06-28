using SociallyDistant.Core.Missions;

namespace SociallyDistant.GameplaySystems.Missions
{
	public sealed class ObjectiveHandle : IObjectiveHandle
	{
		private readonly ObjectiveController objectiveController;

		/// <inheritdoc />
		public string Name
		{
			get => objectiveController.Name;
			set => objectiveController.Name = value;
		}

		/// <inheritdoc />
		public string Description
		{
			get => objectiveController.Description;
			set => objectiveController.Description = value;
		}

		/// <inheritdoc />
		public string? Hint
		{
			get => objectiveController.Hint;
			set => objectiveController.Hint = value;
		}
		
		/// <inheritdoc />
		public bool IsOptionalChallenge
		{
			get => objectiveController.IsOptional;
			set => objectiveController.IsOptional = value;
		}

		public ObjectiveHandle(ObjectiveController controller)
		{
			this.objectiveController = controller;
		}

		/// <inheritdoc />
		public bool IsFAiled => objectiveController.IsFailed;

		/// <inheritdoc />
		public void MarkCompleted()
		{
			if (objectiveController.IsFailed)
				throw new InvalidOperationException("Cannot complete a failed objective.");

			objectiveController.Complete();
		}

		/// <inheritdoc />
		public void MarkFailed(string reason)
		{
			if (objectiveController.IsCompleted)
				throw new InvalidOperationException("Cannot fail a completed objective.");

			objectiveController.Fail(reason);
		}
	}
}