#nullable enable
using Social;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public sealed class TypingIndicatorAvatar : MonoBehaviour
	{
		[SerializeField]
		private RawImage avatar = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TypingIndicatorAvatar));
		}

		public void SetProfile(IProfile profile)
		{
			
		}
	}
}