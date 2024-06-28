#nullable enable

namespace SociallyDistant.Core
{
	public class MarkupAsset
	{
		private string markup = string.Empty;

		public string Markup => markup;
		
		#if UNITY_EDITOR
		
		public void SetMarkup(string markup)
		{
			this.markup = markup;
		}		

#endif
	}
}