using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Visual.UI;
using Com.TheFallenGames.OSA.Core;
using System;
using UnityEngine.Events;

namespace Com.TheFallenGames.OSA.Util
{
    /// <summary>
    /// Utility providing a way of getting the currently focused item by a <see cref="Snapper8"/> on OSA. 
    /// Attach it to the same game object containing your OSA implementation.
    /// </summary>
	public class OSASnapperFocusedItemInfo : MonoBehaviour
    {
        [SerializeField]
        Snapper8 _Snapper = null;

        /// <summary>Fired when the currently focused item changes. It passes the actual ViewsHolder instance as <see cref="AbstractViewsHolder"/> - simply cast it to your known VH type, if needed</summary>
        public FocusedItemChangedUnityEvent FocusedItemChanged;

        /// <summary>Fired when the currently focused item changes</summary>
        public FocusedItemIndexChangedUnityEvent FocusedItemIndexChanged;

        /// <summary>
        /// The "ItemIndex" of the focused item, if any. -1 if none focused
        /// </summary>
        public int FocusedIndex { get { return _FocusedIndex; } }

        int _FocusedIndex = -1;
        IOSA _OSA;


        void Start()
		{
            _OSA = GetComponent(typeof(IOSA)) as IOSA;
			_OSA.ItemsRefreshed += OnItemsRefreshed;
        }

		void Update()
        {
            float _;
            var vh = _Snapper.GetMiddleVH(out _);
            int newIndex;
            if (vh == null)
                newIndex = -1;
            else
                newIndex = vh.ItemIndex;

            if (_FocusedIndex != newIndex)
                ChangeFocusedItem(vh);
        }

        void OnItemsRefreshed(int _, int __)
        {
            if (_FocusedIndex != -1)
                ChangeFocusedItem(null);
        }

        void ChangeFocusedItem(AbstractViewsHolder vh)
        {
            if (vh == null)
                _FocusedIndex = -1;
            else
                _FocusedIndex = vh.ItemIndex;

            // Normally, it's only 1 item with this index, so this is called only once in the loop
            if (FocusedItemChanged != null)
                FocusedItemChanged.Invoke(vh);

            if (FocusedItemIndexChanged != null)
                FocusedItemIndexChanged.Invoke(_FocusedIndex);
        }


        [Serializable]
        public class FocusedItemChangedUnityEvent : UnityEvent<AbstractViewsHolder>
        {

        }


        [Serializable]
        public class FocusedItemIndexChangedUnityEvent : UnityEvent<int>
        {

        }
    }
}