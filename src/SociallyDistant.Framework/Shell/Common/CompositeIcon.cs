#nullable enable

using Microsoft.Xna.Framework.Graphics;

namespace SociallyDistant.Core.Shell.Common
{
	[Serializable]
	public struct CompositeIcon
	{
		public string textIcon;
		public Texture2D? spriteIcon;
		public ShellColor iconColor;
		
		public static implicit operator CompositeIcon(string unicodeTextIcon)
		{
			return new CompositeIcon
			{
				textIcon = unicodeTextIcon,
				spriteIcon = null,
				iconColor = new ShellColor(1, 1, 1, 1)
			};
		}
	}
}