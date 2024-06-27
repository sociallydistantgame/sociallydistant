#nullable enable

using UnityEngine;

namespace Core.Initialization
{
	public abstract class InitializationHook : ScriptableObject
	{
		[SerializeField]
		private int priority;

		public int Priority => priority;

		public abstract void RunHook();
	}
}