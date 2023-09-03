using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using frame8.Logic.Misc.Other.Extensions;

namespace Com.TheFallenGames.OSA.Demos.Common
{
    /// <summary>Smoothly fades back and forth between color a and b</summary>
    public class SimpleImageColorBouncer : MonoBehaviour
    {
        public Color a, b;

        [Range(.001f, 10f)] [SerializeField]
        float _PeriodInSeconds = .3f;

        Graphic _Graphic;


        void Start() { _Graphic = GetComponent<Graphic>(); }


        void Update()
        {
            _Graphic.color = Color.Lerp(a, b, (Mathf.Sin(Time.unscaledTime / _PeriodInSeconds) + 1) / 2);
        }
    }
}
