using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using frame8.Logic.Misc.Other.Extensions;
using UnityEngine.EventSystems;

namespace Com.TheFallenGames.OSA.Util
{
    /// <summary>
    /// Utility to delegate the "long click" event to <see cref="longClickListener"/>
    /// It requires a graphic component (can be an image with zero alpha) that can be clicked in order to receive OnPointerDown, OnPointerUp etc.
    /// No other UI elements should be on top of this one in order to receive pointer callbacks
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    public class LongClickableItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ICancelHandler
    {
        public float longClickTime = .7f;

        public IItemLongClickListener longClickListener;
		public StateEnum State { get { return _State; } }

		float _PressedTime;
		StateEnum _State;
		//int _PointerID;


		public enum StateEnum
		{
			NOT_PRESSING,
			PRESSING_WAITING_FOR_LONG_CLICK,
			PRESSING_AFTER_LONG_CLICK
		}


        void Update()
        {
            if (_State == StateEnum.PRESSING_WAITING_FOR_LONG_CLICK)
            {
                if (Time.unscaledTime - _PressedTime >= longClickTime)
                {
					_State = StateEnum.PRESSING_AFTER_LONG_CLICK;
                    if (longClickListener != null)
                        longClickListener.OnItemLongClicked(this);
                }
            }
        }

        #region Callbacks from Unity UI event handlers
        public void OnPointerDown(PointerEventData eventData)
		{
			//Debug.Log("OnPointerDown" + eventData.button);
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			//_PointerID = eventData.pointerId;

			_State = StateEnum.PRESSING_WAITING_FOR_LONG_CLICK;
            _PressedTime = Time.unscaledTime;
        }
        public void OnPointerUp(PointerEventData eventData)
		{
			//Debug.Log("OnPointerUp" + eventData.button);
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			_State = StateEnum.NOT_PRESSING;
		}
        public void OnCancel(BaseEventData eventData)
		{
			//Debug.Log("OnCancel");
			_State = StateEnum.NOT_PRESSING;
		}
        #endregion

        /// <summary>Interface to implement by the class that'll handle the long click events</summary>
        public interface IItemLongClickListener
        {
            void OnItemLongClicked(LongClickableItem longClickedItem);
        }
    }
}