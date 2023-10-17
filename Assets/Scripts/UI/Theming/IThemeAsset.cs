#nullable enable
using System.Threading.Tasks;
using ContentManagement;
using UnityEngine;

namespace UI.Theming
{
	public interface IThemeAsset : IGameContent
	{
		bool CanEdit { get; }
		bool CanCopy { get; }
		string Name { get; }
		string Author { get; }
		string Description { get; }
		Texture2D? PreviewImage { get; }

		Task<OperatingSystemTheme> LoadAsync();
	}
}