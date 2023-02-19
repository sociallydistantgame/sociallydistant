#nullable enable

using System.Collections.Generic;

namespace Core.Systems
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

		public int GetNextValue()
		{
			while (usedValues.Contains(nextValue))
				nextValue++;

			usedValues.Add(nextValue);
			return nextValue;
		}
	}
}