#nullable enable
using System;
using System.Collections;
using Architecture;
using UnityEngine;
using System.Threading.Tasks;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Sleep")]
	public class SleepCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			if (Arguments.Length < 1)
			{
				Console.WriteLine("sleep: usage: sleep <milliseconds>");
				EndProcess();
				return Task.CompletedTask;
			}

			if (!int.TryParse(Arguments[0], out int milliseconds))
			{
				Console.WriteLine("sleep: Unexpected time value " + Arguments[0]);
				EndProcess();
				return Task.CompletedTask;
			}
			
			// Create a gameobject in the scene to handle the actual sleep routine.
			var gameObject = new GameObject($"sleep {Arguments[0]}ms");
			
			// Add the Sleeper component to it.
			SleeperComponent sleeper = gameObject.AddComponent<SleeperComponent>();
			
			// Tell the sleeper to run the sleep operation and execute our EndProcess method
			sleeper.Completed = EndProcess;
			sleeper.Sleep(milliseconds);
			
			return Task.CompletedTask;
		}

		private class SleeperComponent : MonoBehaviour
		{
			public Action? Completed;

			public void Sleep(int ms)
			{
				float seconds = ms / 1000f;
				StartCoroutine(SleepCoroutine(seconds));
			}

			private IEnumerator SleepCoroutine(float seconds)
			{
				yield return new WaitForSeconds(seconds);
				Completed?.Invoke();
				Completed = null;
				Destroy(this.gameObject);
			}
		}
	}
}