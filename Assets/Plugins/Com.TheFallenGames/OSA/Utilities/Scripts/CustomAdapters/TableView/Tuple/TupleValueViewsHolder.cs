using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Input;
using frame8.Logic.Misc.Other.Extensions;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple
{
	public abstract class TupleValueViewsHolder : BaseItemViewsHolder
	{
		public bool HasPendingTransversalSizeChanges { get; set; }
		public ContentSizeFitter CSF { get { return _CSF; } }
		//public LayoutGroup LayoutGroup { get { return _LayoutGroup; } }
		public TableViewText TextComponent { get { return _TextComponent; } }

		TableViewText _TextComponent;
		Button _Button;
		ContentSizeFitter _CSF;
		//LayoutGroup _LayoutGroup;
		UnityAction<object> _ValueChangedFromInput;


		public override void CollectViews()
		{
			base.CollectViews();

			_Button = root.GetComponent<Button>();
			_CSF = root.GetComponent<UnityEngine.UI.ContentSizeFitter>();
			//_LayoutGroup = root.GetComponent<UnityEngine.UI.LayoutGroup>();
			root.GetComponentAtPath("TextPanel/Text", out _TextComponent);
		}

		public virtual void SetClickListener(UnityAction action)
		{
			if (_Button)
			{
				if (action == null)
					_Button.onClick.RemoveAllListeners();
				else
					_Button.onClick.AddListener(action);
			}
		}

		public virtual void SetValueChangedFromInputListener(UnityAction<object> action)
		{
			_ValueChangedFromInput = action;
		}

		public abstract void UpdateViews(object value, ITableColumns columnsProvider);

		/// <summary>
		/// Called by the controller of this Views Holder, when a click is not handled by it and should be processed by this Views Holder itself
		/// </summary>
		public virtual void ProcessUnhandledClick()
		{

		}

		public override void MarkForRebuild()
		{
			// Don't LayoutRebuilder.MarkLayoutForRebuild(), because the tuples in a TableView are rebuilt 
			// via LayoutRebuilder.ForceRebuildLayoutImmediate() by the TupleAdapter itself
			//base.MarkForRebuild();

			if (CSF)
				CSF.enabled = true;
		}

		public override void UnmarkForRebuild()
		{
			if (CSF)
				CSF.enabled = false;

			base.UnmarkForRebuild();
		}

		protected void NotifyValueChangedFromInput(object newValue)
		{
			if (_ValueChangedFromInput != null)
				_ValueChangedFromInput(newValue);
		}
	}
}
