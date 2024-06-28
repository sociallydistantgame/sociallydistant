#nullable enable

namespace SociallyDistant.Core.Core.Systems
{
	public class UniqueIntGenerator
	{
		private readonly List<int> usedValues = new List<int>();
		private int nextValue;

		public void DeclareUnused(int value)
		{
			if (!usedValues.Contains(value))
				return;

			usedValues.Remove(value);
			nextValue = value;
		}

		public void ClaimUsedValue(int value)
		{
			if (usedValues.Contains(value))
				return;

			usedValues.Add(value);

			while (usedValues.Contains(nextValue))
				nextValue++;
		}

		public int GetNextValue()
		{
			while (usedValues.Contains(nextValue))
				nextValue++;

			ClaimUsedValue(nextValue);
			return nextValue;
		}
	}
}