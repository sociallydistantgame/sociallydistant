#nullable enable

using System;
using System.Linq;
using UnityEngine;

namespace Core.Initialization
{
	public class InitializationHookRunner : MonoBehaviour
	{
		private InitializationHook[] hooks = null!;
		
		[Header("Configuration")]
		[SerializeField]
		private string hookSearchPath = "InitializationHooks";

		private void Awake()
		{
			Debug.Log("Finding init hooks...");
			this.hooks = Resources.LoadAll<InitializationHook>(hookSearchPath);
		}

		private void Start()
		{
			foreach (InitializationHook hook in hooks.OrderByDescending(x => x.Priority))
				hook.RunHook();
		}
	}
}