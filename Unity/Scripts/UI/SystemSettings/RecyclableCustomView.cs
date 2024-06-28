#nullable enable
using UnityEngine;
using UnityExtensions;

namespace UI.SystemSettings
{
	public class RecyclableCustomView : MonoBehaviour
	{
		
		private RectTransform viewsRect = null!;

		public RectTransform ViewsRect => viewsRect;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(RecyclableCustomView));
		}
	}
}