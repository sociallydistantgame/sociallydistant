#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using GamePlatform.ContentManagement;

namespace GamePlatform
{
	public class LocalGameDataSource : IGameContentSource
	{
		/// <inheritdoc />
		public async Task LoadAllContent(ContentCollectionBuilder builder)
		{
			string baseDirectory = LocalGameData.BaseDirectory;

			if (!Directory.Exists(baseDirectory))
				Directory.CreateDirectory(baseDirectory);

			foreach (string userDirectory in Directory.GetDirectories(baseDirectory))
			{
				LocalGameData? localData = await LocalGameData.TryLoadFromDirectory(userDirectory);
				if (localData != null)
					builder.AddContent(localData);
			}
		}
	}
}