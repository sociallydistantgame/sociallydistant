#nullable enable
using System;
using System.Collections;
using Architecture;
using UnityEngine;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Sleep")]
	public class SleepCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override void OnExecute()
		{
			if (Arguments.Length < 1)
			{
				Console.WriteLine("sleep: usage: sleep <milliseconds>");
				EndProcess();
				return;
			}

			if (!int.TryParse(Arguments[0], out int milliseconds))
			{
				Console.WriteLine("sleep: Unexpected time value " + Arguments[0]);
				EndProcess();
				return;
			}
			
			// Create a gameobject in the scene to handle the actual sleep routine.
			var gameObject = new GameObject($"sleep {Arguments[0]}ms");
			
			// Add the Sleeper component to it.
			SleeperComponent sleeper = gameObject.AddComponent<SleeperComponent>();
			
			// Tell the sleeper to run the sleep operation and execute our EndProcess method
			sleeper.Completed = EndProcess;
			sleeper.Sleep(milliseconds);
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