using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;

#if OSA_TV_TMPRO
using TText = TMPro.TextMeshProUGUI;
#else
using TText = UnityEngine.UI.Text;
#endif

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Input
{
	/// <summary>
	/// Wrapper around a Text or a TMPro.TextMeshProUGUI.
	/// It expects a Text component attached. 
	/// Or its TMPro counterpart if OSA_TV_TMPRO is defined
	/// </summary>
	public class TableViewText : MonoBehaviour
	{
		[Tooltip(
			"Only needed if you want to display something like an ellipsis when the text shown isn't the entire text. " +
			"If you're using TMPro, you won't need this, as TMPro already has an ellipsis adding mechanism built-in")]
		[SerializeField]
		Transform _ObjectToActivateOnOverflow = null;

		/// <summary>
		/// Keeping the same property name as the Unity's Text component
		/// </summary>
		public string text
		{
			get { return _Text.text; }
			set { _Text.text = value; }
		}

		/// <summary>
		/// Keeping the same property name as the Unity's Text component
		/// </summary>
		public bool supportRichText
		{
#if OSA_TV_TMPRO
			get { return _Text.richText; }
			set { _Text.richText = value; }
#else
			get { return _Text.supportRichText; }
			set { _Text.supportRichText = value; }
#endif
		}

		/// <summary>
		/// Keeping the same property name as the Unity's Text component
		/// </summary>
		public int fontSize
		{
#if OSA_TV_TMPRO
			get { return (int)(_Text.fontSize + .5f); /*rounding to nearest int*/ }
#else
			get { return _Text.fontSize; }
#endif
			set { _Text.fontSize = value; }
		}

		/// <summary>
		/// Keeping the same property name as the Unity's Text component
		/// </summary>
		public Color color
		{
			get { return _Text.color; }
			set { _Text.color = value; }
		}

		public RectTransform RT
		{
			get
			{
				if (!_RetrievedRT)
				{
					_RetrievedRT = true;
					_RT = transform as RectTransform;
				}

				return _RT;
			}
		}

		bool _RetrievedRT;
		RectTransform _RT;
		TText _Text;
		//float _SavedAlpha;


		void OnEnable()
		{
			if (_Text)
				_Text.enabled = true;
		}

		void Awake()
		{
			_Text = GetComponent<TText>();
			if (!_Text)
				throw new OSAException("TableViewText: no " + typeof(TText).Name + " component found (expecting it because OSA_TV_TMPRO scripting symbol is defined)");

//			// The builtin Text will disappear sometimes if verticalOverflow is not set to VerticalWrapMode.Overflow
//#if OSA_TV_TMPRO
//			_Text.overflowMode = TMPro.TextOverflowModes.Ellipsis;
//#else
//			//_Text.verticalOverflow = VerticalWrapMode.Overflow;
//			if (_ObjectToActivateOnOverflow)
//				SetOverflowActive(false);
//#endif
			if (_ObjectToActivateOnOverflow)
				SetOverflowActive(false);
		}

		// Manually add an Ellipsis if the text overflows, when using the built-in Text component
		void Update()
		{
			// Update at larger intervals, for better performance
			if (Time.frameCount % 10 == 0)
				CheckOverflow();
		}

		void CheckOverflow()
		{
			if (!_Text || !_ObjectToActivateOnOverflow)
				return;

			bool active = false;
#if OSA_TV_TMPRO
			active = _Text.isTextOverflowing;
#else
			var textGen = _Text.cachedTextGenerator;
			if (textGen != null)
				active = textGen.characterCountVisible != _Text.text.Length;
#endif
			SetOverflowActive(active);
		}

		void SetOverflowActive(bool overFlowActive)
		{
			//float scaleToSet = overFlowActive ? 1f : 0f;
			//if (_ObjectToActivateOnOverflow.localScale.x == scaleToSet)
			//	return;

			//var l = _ObjectToActivateOnOverflow.localScale;
			//l.x = scaleToSet;
			//_ObjectToActivateOnOverflow.localScale = l;

			_ObjectToActivateOnOverflow.gameObject.SetActive(overFlowActive);
		}


			void OnDisable()
		{
			if (_Text)
				_Text.enabled = false;
		}

		//public void SetEnabledByScalingGameObject(bool enabled)
		//{
		//	transform.localScale = enabled ? Vector3.one : Vector3.zero;
		//}

		/// <summary>Returns the previous alpha</summary>
		public float SetAlpha(float alpha)
		{
			float prevAlpha;
			if (_Text)
			{
				var c = _Text.color;
				prevAlpha = c.a;
				c.a = alpha;
				_Text.color = c;
			}
			else
				prevAlpha = 0f;

			return prevAlpha;
		}
	}
}
