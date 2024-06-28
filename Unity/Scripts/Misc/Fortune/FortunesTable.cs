#nullable enable

using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Misc.Fortune
{
	[CreateAssetMenu(menuName = "ScriptableObject/Miscellaneous/Fortunes Table")]
	public class FortunesTable : ScriptableObject
	{
		
		private string[] fortunes = Array.Empty<string>();

		public string GetRandomFortune()
		{
			if (fortunes.Length == 0)
				return "You have no fortune.";

			return fortunes[Random.Range(0, fortunes.Length)];
		}
		
		#if UNITY_EDITOR
		public void SetFortunesList(string[] fortunesList)
		{
			EditorUtility.SetDirty(this);
			this.fortunes = fortunesList;
		}
		#endif
	}
}