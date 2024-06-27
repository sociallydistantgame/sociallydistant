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

		[SerializeField]
		private Material avatarMaterial = null!;
		
		private Material? materialInstance = null!;
		
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
				humanColor = value;
				UpdateUI();
			}
		}
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DefaultAvatarRenderer));
			UpdateUI();
		}

		private void OnDestroy()
		{
			if (materialInstance != null)
				Destroy(materialInstance);
		}

		private void UpdateUI()
		{
			ring.ShapeProperties.OutlineColor = humanColor;
			ring.ForceMeshUpdate();

			if (materialInstance == null)
				materialInstance = Instantiate(avatarMaterial);

			materialInstance.SetColor("_BackgroundColor", useDarkMode ? Color.white : Color.black);
			materialInstance.SetColor("_ForegroundColor", humanColor);
			materialInstance.SetColor("_Color", image.color);
			
			image.material = materialInstance;
		}
	}
}