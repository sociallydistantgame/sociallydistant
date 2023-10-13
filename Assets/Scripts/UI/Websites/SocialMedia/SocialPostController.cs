#nullable enable
using UnityEngine;
using UnityExtensions;

namespace UI.Websites.SocialMedia
{
	public class SocialPostController : MonoBehaviour
	{
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SocialPostController));
		}

		public void SetData(SocialPostModel model)
		{
			
		}
	}
}