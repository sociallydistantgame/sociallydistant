using System.Collections;
using System.Collections.Generic;
using Core;
using Modules;

namespace Shell
{
	public class TabbedToolCollection : ICollection<ITabbedToolDefinition>
	{
		private readonly List<ITabbedToolDefinition> tools = new List<ITabbedToolDefinition>();
		private readonly IGameContext context;

		public TabbedToolCollection(IGameContext context)
		{
			this.context = context;
		}
		
		/// <inheritdoc />
		public IEnumerator<ITabbedToolDefinition> GetEnumerator()
		{
			return tools.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(ITabbedToolDefinition item)
		{
			tools.Add(item);
		}

		/// <inheritdoc />
		public void Clear()
		{
			tools.Clear();
		}

		/// <inheritdoc />
		public bool Contains(ITabbedToolDefinition item)
		{
			return tools.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(ITabbedToolDefinition[] array, int arrayIndex)
		{
			tools.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(ITabbedToolDefinition item)
		{
			return tools.Remove(item);
		}

		/// <inheritdoc />
		public int Count => tools.Count;

		/// <inheritdoc />
		public bool IsReadOnly => context.CurrentGameMode == GameMode.OnDesktop;
	}
}