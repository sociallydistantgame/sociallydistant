#nullable enable
using SociallyDistant.Core.Core;

namespace SociallyDistant.Architecture
{
	public class LifepathAsset : INamedAsset
	{
		private string uniqueId = string.Empty;
		private string lifepathName = string.Empty;
		private string description = "";

		/// <inheritdoc />
		public string Name => uniqueId;

		public string LifepathName => lifepathName;

		public string Description => description;

		public LifepathAsset(string id, string name, string description)
		{
			this.uniqueId = id;
			this.lifepathName = name;
			this.description = description;
		}
	}
}