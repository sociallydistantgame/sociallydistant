using System;
using Architecture;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityExtensions;

namespace UI.CharacterCreator
{
	public class LifepathView : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField]
		private Image previewImage = null!;

		[SerializeField]
		private TextMeshProUGUI title = null!;

		[SerializeField]
		private TextMeshProUGUI description = null!;

		private Toggle toggle;
        
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(LifepathView));
			this.MustGetComponent(out toggle);
			this.MustGetComponentInParent(out ToggleGroup group);
			this.toggle.group = group;
		}

		public void SetData(LifepathAsset data)
		{
			title.SetText(data.LifepathName);
			description.SetText(data.Description);
		}
	}
}