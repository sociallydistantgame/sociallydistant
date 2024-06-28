#nullable enable

using System.Collections;

namespace SociallyDistant.Architecture.AssetTypes
{
	public class StringList : IReadOnlyList<string>
	{
		private string[] entries = Array.Empty<string>();
		
		public string GetRandomEntry()
		{
			if (entries.Length == 0)
				return string.Empty;

			// TODO: Global rng
			return entries[0];
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