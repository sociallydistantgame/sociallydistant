using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;

namespace Com.TheFallenGames.OSA.Demos.Common
{
    /// <summary>Uses GUILayout to display a FPS counter in middle-top</summary>
    public class SimpleFPSCounter : MonoBehaviour
	{
		[SerializeField]
		Text _Text;

		float deltaTime = 0.0f;
		public bool setTargetFPSTo60 = true;


		void Start()
		{
			if (setTargetFPSTo60)
				Application.targetFrameRate = 60;

			if (!_Text)
			{
				var cs = FindObjectsOfType<Canvas>();

				Canvas cToChoose = null;
				foreach (var c in cs)
				{
					if (c.renderMode == RenderMode.ScreenSpaceCamera || c.renderMode == RenderMode.ScreenSpaceOverlay)
					{
						cToChoose = c;
						break;
					}
				}
				if (!cToChoose && cs.Length > 0)
					cToChoose = cs[0]; // better a world-space canvas than nothing

				if (cToChoose)
				{
					var goRT = new GameObject("FPSCounter", typeof(RectTransform)).transform as RectTransform;
					goRT.SetParent(cToChoose.transform as RectTransform, false);
					goRT.MatchParentSize(false);
					var t = goRT.gameObject.AddComponent<Text>();
					t.alignment = TextAnchor.UpperCenter;
					t.color = Color.white;
					t.fontSize = 15;
					string fontName;
#if UNITY_2022_2_OR_NEWER
					fontName = "LegacyRuntime.ttf";
#else
					fontName = "Arial.ttf";
#endif
					t.font = Resources.GetBuiltinResource<Font>(fontName);
					//t.gameObject.AddComponent<Shadow>();
					t.supportRichText = false;
					t.raycastTarget = false;

					_Text = t;
				}
			}
		}

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

			// Once every 3 frames
			if (Time.frameCount % 3 != 0)
				return;

			float msec = deltaTime * 1000.0f;
			float fps = 1.0f / deltaTime;
			string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

			if (_Text)
				_Text.text = text;
		}
    }
}