//#define OSA_TV_TMPRO


using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using frame8.Logic.Misc.Other.Extensions;
using UnityEngine.UI;

#if OSA_TV_TMPRO
using TInputField = TMPro.TMP_InputField;
#else
using TInputField = UnityEngine.UI.InputField;
#endif

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Input
{
	/// <summary>
	/// It expects an InputField in the direct children, and a Text. 
	/// Or their TMPro counterparts if OSA_TV_TMPRO is defined
	/// </summary>
	public class TableViewTextInputController : MonoBehaviour
	{
		string text
		{
			get { return _InputField.text; }
			set
			{
				_InputField.text = value;
				UpdateSizeControllerText(_InputField.text);
			}
		}

		/// <summary>
		/// Keeping the same property name as the Unity's Text component
		/// </summary>
		public int fontSize
		{
			get { return _Text.fontSize; }
			set
			{
				_Text.fontSize = value;
				_InputField.textComponent.fontSize = value;
			}
		}

		bool interactable { get { return _InputField.interactable; } set { _InputField.interactable = value; } } 
		//public TText textComponent { get { return _InputField.textComponent; } /*set { _InputField.textComponent = value; }*/ } 
		//public Image image { get { return _InputField.image; } /*set { _InputField.image = value; }*/ } 

		bool MultiLine
		{
			get { return _InputField.multiLine; }
			set
			{
				_InputField.lineType = value ? TInputField.LineType.MultiLineNewline : TInputField.LineType.SingleLine;
			}
		}

		//public bool CanAcceptInput { get { return isActiveAndEnabled && interactable && _InputField.isActiveAndEnabled; } }

		RectTransform _RT;
		TInputField _InputField;
		TableViewText _Text;
		LayoutElement _TextLayoutElement;
		//LayoutElement _MyLayoutElement;
		Action<string> _CurrentEndEditCallback;
		Action _CurrentCancelCallback;


		//void OnEnable()
		//{
		//	if (_InputField)
		//		_InputField.enabled = true;
		//	if (_Text)
		//		_Text.enabled = true;
		//}

		void Awake()
		{
			_RT = transform as RectTransform;
			_RT.pivot = new Vector2(0f, 1f); // top-left

			for (int i = 0; i < transform.childCount; i++)
			{
				var ch = transform.GetChild(i);
				if (!_InputField)
					_InputField = ch.GetComponent<TInputField>();
				if (!_Text)
					_Text = ch.GetComponent<TableViewText>();
			}

			if (!_InputField)
				throw new OSAException("TableView: no " + typeof(TInputField).Name + " component found in direct children");

			if (!_Text)
				throw new OSAException("TableView: no " + typeof(TableViewText).Name + " component field found in direct children");

			if (!_InputField.textComponent)
				throw new OSAException("TableView: the " + typeof(TInputField).Name + " has no text component specified");

			var layEl = _InputField.GetComponent<LayoutElement>();
			if (!layEl)
				layEl = _InputField.gameObject.AddComponent<LayoutElement>();

			layEl.ignoreLayout = true;
			var rt = layEl.transform as RectTransform;
			rt.MatchParentSize(true);

			_TextLayoutElement = _Text.GetComponent<LayoutElement>();
			if (!_TextLayoutElement)
				_TextLayoutElement = _Text.gameObject.AddComponent<LayoutElement>();
			_TextLayoutElement.preferredHeight = _TextLayoutElement.preferredWidth = -1f;
			_TextLayoutElement.flexibleHeight = _TextLayoutElement.flexibleWidth = -1f;

			//layEl.flexibleHeight = _FlexibleHeight;
			//layEl.flexibleWidth = _FlexibleWidth;

			var group = GetComponent<HorizontalLayoutGroup>();
			if (!group)
				group = gameObject.AddComponent<HorizontalLayoutGroup>();

			group.childForceExpandHeight = group.childForceExpandWidth = false;
			group.childControlHeight = group.childControlWidth = true;

			//_MyLayoutElement = GetComponent<LayoutElement>();
			//if (!_MyLayoutElement)
			//	_MyLayoutElement = gameObject.AddComponent<LayoutElement>();

			var csf = GetComponent<ContentSizeFitter>();
			if (!csf)
				csf = gameObject.AddComponent<ContentSizeFitter>();
			csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			_InputField.onValueChanged.AddListener(UpdateSizeControllerText);
			_InputField.onEndEdit.AddListener(OnEndEdit);
		}

		//void OnDisable()
		//{
		//	if (_InputField)
		//		_InputField.enabled = false;
		//	if (_Text)
		//		_Text.enabled = false;
		//}

		void OnDestroy()
		{
			if (_InputField)
				_InputField.onValueChanged.RemoveListener(UpdateSizeControllerText);
		}

		public void ShowFloating(RectTransform atParent, string initialText, bool multiLine, Action<string> onEndEdit, Action onCancel)
		{
			_CurrentEndEditCallback = null;
			_CurrentCancelCallback = null;
			gameObject.SetActive(true);
			_CurrentEndEditCallback = onEndEdit;
			_CurrentCancelCallback = onCancel;

			var parRect = atParent.rect;

			_RT.position = atParent.position;

			_TextLayoutElement.minWidth = _TextLayoutElement.preferredWidth = parRect.width;
			_TextLayoutElement.minHeight = _TextLayoutElement.preferredHeight = parRect.height;
			//_RT.SetSizeFromParentEdgeWithCurrentAnchors(_RT.parent as RectTransform, RectTransform.Edge.Left, parRect.width);
			_RT.TryClampPositionToParentBoundary();

			MultiLine = multiLine;

			ActivateInputField();

			text = initialText;
		}

		public void Hide()
		{
			_CurrentEndEditCallback = null;


			var cancelCallback = _CurrentCancelCallback;
			bool callCancel = false;
			_CurrentCancelCallback = null;
			if (_InputField && _InputField.isActiveAndEnabled && _InputField.isFocused)
			{
				DeactivateInputField();
				callCancel = true;
			}

			gameObject.SetActive(false);

			if (callCancel && cancelCallback != null)
				cancelCallback();
		}


		void ActivateInputField()
		{
			_InputField.ActivateInputField();
		}
		void DeactivateInputField()
		{
			_InputField.DeactivateInputField();
		}

		void UpdateSizeControllerText(string _)
		{
			_Text.text = _InputField.text;
		}

		void OnEndEdit(string text)
		{
			var c = _CurrentEndEditCallback;

			Hide();

			if (c != null)
				c(text);
		}
	}
}
