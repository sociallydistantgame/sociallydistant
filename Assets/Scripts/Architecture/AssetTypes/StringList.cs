#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture.AssetTypes
{
	public class StringList : 
		ScriptableObject,
		IReadOnlyList<string>
	{
		[Multiline]
		[SerializeField]
		private string[] entries = Array.Empty<string>();
		
		public string GetRandomEntry()
		{
			if (entries.Length == 0)
				return string.Empty;

			return entries[UnityEngine.Random.Range(0, entries.Length)];
		}
		
		#if UNITY_EDITOR

		public void SetEntries(string[] entryData)
		{
			this.entries = entryData;
		}
		
		#endif
		/// <inheritdoc />
		public IEnumerator<string> GetEnumerator()
		{
			foreach (string entry in entries)
				yield return entry;
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public int Count => entries.Length;

		/// <inheritdoc />
		public string this[int index] => entries[index];
	}
}