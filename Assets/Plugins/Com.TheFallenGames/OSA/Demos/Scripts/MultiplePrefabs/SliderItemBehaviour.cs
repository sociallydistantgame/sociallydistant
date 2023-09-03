using System;
using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.Models;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.ViewsHolders;

namespace Com.TheFallenGames.OSA.Demos.MultiplePrefabs
{
    /// <summary>
    /// <para>Basic behavior used on items related to <see cref="BidirectionalVH"/> and <see cref="BidirectionalModel"/> to demonstrate the bidirectional flow of data (i.e. form model to the view and vice-versa)</para>
    /// <para>It fires its <see cref="ValueChanged"/> when the slider's value changes and displays it in a Text component. The slider's value can also be changed/retrieved via the <see cref="Value"/> property</para>
    /// </summary>
    [ExecuteInEditMode]
    public class SliderItemBehaviour : MonoBehaviour
    {
        public event Action<float> ValueChanged;
        /// <summary>Gets/Sets the value of the slider</summary>
        public float Value { get { return _Slider.value; } set { _Slider.value = value; } }

        Text _Value;
        Slider _Slider;


        void Awake()
        {
            _Value = transform.Find("ValueText").GetComponentInChildren<Text>();
            _Slider = GetComponentInChildren<Slider>();
            
            // Don't add a listener if in edit mode. Will use Update instead
            if (Application.isPlaying)
            {
                _Slider.onValueChanged.AddListener(OnValueChanged);
                OnValueChanged(_Slider.value);
            }
        }

#if UNITY_EDITOR
        void Update()
        {
            if (!Application.isPlaying)
                OnValueChanged(_Slider.value);
        }
#endif


        void OnValueChanged(float value)
        {
            _Value.text = value.ToString("0.00");
#if UNITY_EDITOR
            // Don't fire ValueChanged if in edit mode
            if (!Application.isPlaying)
                return;
#endif
            if (ValueChanged != null)
                ValueChanged(value);
        }
    }
}
