#nullable enable
using System;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Widgets.Settings
{
	public sealed class DefaultAvatarRenderer : MonoBehaviour
	{
		[SerializeField]
		private Image image = null!;

		[SerializeField]
		private Rectangle ring = null!;

		[SerializeField]
		private bool useDarkMode;
		
		[SerializeField]
		private Color humanColor = Color.blue;

		public bool UseDarkMode
		{
			get => useDarkMode;
			set
			{
				if (useDarkMode == value)
					return;

				useDarkMode = value;
				UpdateUI();
			}
		}

		public Color HumanColor
		{
			get => humanColor;
			set
			{
				if (humanColor == value)
					return;

				humanColor = value;
				UpdateUI();
			}
		}
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DefaultAvatarRenderer));
			
			
			
			UpdateUI();
		}

		private void UpdateUI()
		{
			ring.ShapeProperties.OutlineColor = humanColor;
			ring.ForceMeshUpdate();

			Material avatarMaterial = image.material;

			avatarMaterial.SetColor("_Color", useDarkMode ? Color.white : Color.black);
			avatarMaterial.SetColor("_ForegroundColor", humanColor);
		}

		private void OnValidate()
		{
			UpdateUI();
		}
	}
}