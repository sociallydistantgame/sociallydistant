Avatar#nullable enable
using System;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Widgets.Settings
{
	public sealed class DefaultAvatarRenderer : MonoBehaviour
	{
		
		private Image image = null!;

		
		private Rectangle ring = null!;

		
		private bool useDarkMode;
		
		
		private Color humanColor = Color.blue;

		
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