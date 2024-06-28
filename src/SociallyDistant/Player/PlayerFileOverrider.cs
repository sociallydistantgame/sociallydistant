#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.Player
{
	public sealed class PlayerFileOverrider : IFileOverrider
	{
		private readonly Dictionary<string, int> directoryMap = new Dictionary<string, int>();
		private readonly Dictionary<int, List<IFileEntry>> directoryLists = new Dictionary<int, List<IFileEntry>>();

		/// <inheritdoc />
		public bool TryGetFile(IUser user, ReadOnlySpan<string> directory, string filename, out IFileEntry? entry)
		{
			entry = null;
			string directoryPath = PathUtility.Combine(directory.ToArray());

			if (!directoryMap.TryGetValue(directoryPath, out int id))
				return false;

			if (!directoryLists.TryGetValue(id, out List<IFileEntry>? fileList))
			{
				directoryMap.Remove(directoryPath);
				return false;
			}

			entry = fileList.FirstOrDefault(x => x.Name == filename);
			return entry != null;
		}

		/// <inheritdoc />
		public IEnumerable<string> EnumerateFiles(IUser ser, string[] directory)
		{
			string directoryPath = PathUtility.Combine(directory.ToArray());

			if (!directoryMap.TryGetValue(directoryPath, out int id))
				yield break;
			
			if (!directoryLists.TryGetValue(id, out List<IFileEntry>? fileList))
			{
				directoryMap.Remove(directoryPath);
				yield break;
			}

			foreach (IFileEntry entry in directoryLists[id])
				yield return entry.Name;
		}

		/// <inheritdoc />
		public void AddFile(ReadOnlySpan<string> directory, IFileEntry entry)
		{
			string directoryPath = PathUtility.Combine(directory.ToArray());

			if (!directoryMap.TryGetValue(directoryPath, out int id))
			{
				id = GetNextAvailableId();
				directoryMap.Add(directoryPath, id);
			}
			
			if (!directoryLists.TryGetValue(id, out List<IFileEntry>? fileList))
			{
				directoryLists.Add(id, new List<IFileEntry>());
			}

			directoryLists[id].Add(entry);
		}

		/// <inheritdoc />
		public void RemoveFile(ReadOnlySpan<string> directory, IFileEntry entry)
		{
			string directoryPath = PathUtility.Combine(directory.ToArray());

			if (!directoryMap.TryGetValue(directoryPath, out int id))
				return;
			
			if (!directoryLists.TryGetValue(id, out List<IFileEntry>? fileList))
			{
				directoryMap.Remove(directoryPath);
				return;
			}

			directoryLists[id].Remove(entry);
			if (directoryLists[id].Count == 0)
			{
				directoryLists.Remove(id);
				directoryMap.Remove(directoryPath);
			}
		}

		private int GetNextAvailableId()
		{
			var i = 0;
			while (directoryLists.ContainsKey(i))
				i++;
			return i;
		}
	}
}