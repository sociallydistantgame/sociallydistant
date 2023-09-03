using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.Demos.Common
{
	[ExecuteInEditMode]
	public class AspectRatioFitterByLayoutElement : MonoBehaviour
	{
		public AspectRatioFitter.AspectMode aspectMode;
		public float aspectRatio;

		LayoutElement _LayoutElement;
		RectTransform _RT;


		void Awake()
		{
			_LayoutElement = GetComponent<LayoutElement>();
		}

		void OnEnable()
		{
			UpdateAspectRatio();
		}

		void OnRectTransformDimensionsChange()
		{
			if (isActiveAndEnabled)
				UpdateAspectRatio();
		}

		void Update()
		{
			UpdateAspectRatio();
		}

		void UpdateAspectRatio()
		{
			if (!_LayoutElement)
			{
				_LayoutElement = GetComponent<LayoutElement>();
				if (!_LayoutElement)
					return;
			}
			if (!_RT)
				_RT = transform as RectTransform;

			if (aspectRatio == 0f)
				return;

			switch (aspectMode)
			{
				case AspectRatioFitter.AspectMode.HeightControlsWidth:
					float h = _RT.rect.height;
					float newPW = h * aspectRatio;
					if (_LayoutElement.preferredWidth != newPW)
						_LayoutElement.preferredWidth = newPW;

					break;

				case AspectRatioFitter.AspectMode.WidthControlsHeight:
					float w = _RT.rect.width;
					float newPH = w / aspectRatio;
					if (_LayoutElement.preferredHeight != newPH)
						_LayoutElement.preferredHeight = newPH;

					break;
			}
		}
	}
}