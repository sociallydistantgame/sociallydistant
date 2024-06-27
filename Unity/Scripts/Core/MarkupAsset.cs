#nullable enable

using UnityEngine;

namespace Core
{
	public class MarkupAsset : ScriptableObject
	{
		[TextArea]
		[SerializeField]
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