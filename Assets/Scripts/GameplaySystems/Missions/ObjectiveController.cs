using System;

namespace GameplaySystems.Missions
{
	public sealed class ObjectiveController
	{
		private readonly Action updateSignal;
		private string name = string.Empty;
		private string description = string.Empty;
		private string? failReason;
		private bool isOptional;
		private bool failed;
		private bool completed;
		private string? hint;

		public string Name
		{
			get => name;
			set
			{
				if (name == value)
					return;

				name = value;
				updateSignal();
			}
		}

		public string Description
		{
			get => description;
			set
			{
				if (description == value)
					return;
				
				description = value;
				updateSignal();
			}
		}

		public string? Hint
		{
			get => hint;
			set
			{
				if (hint == value)
					return;

				hint = value;
				updateSignal();
			}
		}

		public bool IsOptional
		{
			get => isOptional;
			set
			{
				if (isOptional == value)
					return;

				isOptional = value;
				updateSignal();
			}
		}

		public string? FailReason => failReason;
		public bool IsCompleted => completed;
		public bool IsFailed => failed;
		
		public ObjectiveController(Action updateSignal)
		{
			this.updateSignal = updateSignal;
		}

		public void Complete()
		{
			failReason = null;
			failed = false;
			completed = true;
			updateSignal();
		}
		
		public void Fail(string reason)
		{
			failReason = reason;
			failed = true;
			completed = false;
			updateSignal();
		}
	}
}