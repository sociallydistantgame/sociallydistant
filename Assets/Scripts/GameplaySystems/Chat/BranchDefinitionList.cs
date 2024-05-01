#nullable enable
using System.Collections;
using System.Collections.Generic;
using Chat;

namespace GameplaySystems.Chat
{
	public sealed class BranchDefinitionList : ICollection<IBranchDefinition>
	{
		private readonly List<IBranchDefinition> list = new();
		
		/// <inheritdoc />
		public IEnumerator<IBranchDefinition> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(IBranchDefinition item)
		{
			list.Add(item);
		}

		/// <inheritdoc />
		public void Clear()
		{
			list.Clear();
		}

		/// <inheritdoc />
		public bool Contains(IBranchDefinition item)
		{
			return list.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(IBranchDefinition[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(IBranchDefinition item)
		{
			return list.Remove(item);
		}

		/// <inheritdoc />
		public int Count => list.Count;

		/// <inheritdoc />
		public bool IsReadOnly => false;
	}
}