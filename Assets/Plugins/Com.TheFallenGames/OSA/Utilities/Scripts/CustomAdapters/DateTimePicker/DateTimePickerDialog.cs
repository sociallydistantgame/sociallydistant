using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.CustomAdapters.DateTimePicker
{
	/// <summary>
	/// Implementing multiple adapters to get a generic picker which returns a <see cref="DateTime"/> object.
	/// There are 2 ways of using this script: either simply call <see cref="Show(Action{DateTime})"/>
	/// or drag and drop the prefab from /Resources/Com.TheFallenGames/OSA/DateTimePicker8 into your scene and subscribe to <see cref="OnDateSelected"/>
	/// </summary>
	public class DateTimePickerDialog : MonoBehaviour
	{
		[SerializeField]
		bool _AutoInit = false;
		[SerializeField]
		bool _DisplaySelectedDateAsShort = false;
		[SerializeField]
		bool _DisplaySelectedTimeAsShort = false;
		[Tooltip("If true, animations won't be affected by Time.timeScale")]
		[SerializeField]
		bool _UseUnscaledTime = true;

		public event Action<DateTime> OnDateSelected;

		public DateTimePickerAdapter DayAdapter { get; private set; }
		public DateTimePickerAdapter MonthAdapter { get; private set; }
		public DateTimePickerAdapter YearAdapter { get; private set; }

		public DateTimePickerAdapter HourAdapter { get; private set; }
		public DateTimePickerAdapter MinuteAdapter { get; private set; }
		public DateTimePickerAdapter SecondAdapter { get; private set; }

		public DateTime SelectedValue
		{
			get
			{
				return new DateTime(
					YearAdapter.SelectedValue, MonthAdapter.SelectedValue, DayAdapter.SelectedValue,
					HourAdapter.SelectedValue, MinuteAdapter.SelectedValue, SecondAdapter.SelectedValue
				);
			}
		}

		const float SCROLL_DURATION1 = .2f;
		const float SCROLL_DURATION2 = .35f;
		const float SCROLL_DURATION3 = .5f;
		const float ANIM_DURATION = .25f;
		const float DEFAULT_WIDTH = 660f, DEFAULT_HEIGHT = DEFAULT_WIDTH / 2;

		float AnimElapsedTime01 { get { float t = Mathf.Clamp01((Time - _AnimStartTime) / ANIM_DURATION); return t * t * t * t; } }
		//Vector3 AnimCurrentScale { get { return Vector3.Lerp(_AnimStart, _AnimEnd, AnimElapsedTime01); } }
		float AnimCurrentFloat { get { return Mathf.Lerp(_AnimStart, _AnimEnd, AnimElapsedTime01); } }

		float Time { get { return _UseUnscaledTime ? UnityEngine.Time.unscaledTime : UnityEngine.Time.time; } }

		Transform _DatePanel, _TimePanel;
		Text _SelectedDateText, _SelectedTimeText;
		bool _Initialized;
		DateTime? _DateToInitWith;
		bool _Animating;
		//Vector3 _AnimStart, _AnimEnd;
		float _AnimStart, _AnimEnd;
		float _AnimStartTime;
		Action _ActionOnAnimDone;
		CanvasGroup _CanvasGroup;
		DateTimePickerAdapter[] _AllAdapters = new DateTimePickerAdapter[6];


		public static DateTimePickerDialog Show(Action<DateTime> onSelected) 
		{ 
			return Show(DateTime.Now, onSelected); 
		}

		public static DateTimePickerDialog Show(Action<DateTime> onSelected, string prefabPathInResources) 
		{ 
			return Show(DateTime.Now, onSelected, prefabPathInResources); 
		}

		public static DateTimePickerDialog Show(DateTime startingDate, Action<DateTime> onSelected) 
		{ 
			return Show(startingDate, onSelected, DEFAULT_WIDTH, DEFAULT_HEIGHT); 
		}

		public static DateTimePickerDialog Show(DateTime startingDate, Action<DateTime> onSelected, string prefabPathInResources) 
		{ 
			return Show(startingDate, onSelected, DEFAULT_WIDTH, DEFAULT_HEIGHT, prefabPathInResources); 
		}

		public static DateTimePickerDialog Show(DateTime startingDate, Action<DateTime> onSelected, float width, float height)
		{
			var prefabPathInResources = OSAConst.OSA_PATH_IN_RESOURCES + "/" + typeof(DateTimePickerDialog).Name;
			return Show(startingDate, onSelected, width, height, prefabPathInResources);
		}

		public static DateTimePickerDialog Show(DateTime startingDate, Action<DateTime> onSelected, float width, float height, string prefabPathInResources)
		{
			var go = Resources.Load<GameObject>(prefabPathInResources);
			var picker = (Instantiate(go) as GameObject).GetComponent<DateTimePickerDialog>();
			var c = FindObjectOfType<Canvas>();
			if (!c)
				throw new OSAException(typeof(DateTimePickerDialog).Name + ": no Canvas was found in the scene");
			var canvasRT = c.transform as RectTransform;
			var rt = (picker.transform as RectTransform);
			rt.SetParent(canvasRT, false);
			rt.SetAsLastSibling();
			picker._DateToInitWith = startingDate;
			if (onSelected != null)
				picker.OnDateSelected += onSelected;

			rt.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(RectTransform.Edge.Left, (canvasRT.rect.width - width) / 2, width);
			rt.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(RectTransform.Edge.Top, (canvasRT.rect.height - height) / 2, height);

			return picker;
		}


		void Awake()
		{
			_CanvasGroup = GetComponent<CanvasGroup>();
			_CanvasGroup.alpha = 0f;
			_AnimEnd = 1f;
			_AnimStartTime = Time;
			_Animating = true;
		}

		void Start()
		{
			var adaptersTR = transform.Find("Adapters");
			_DatePanel = adaptersTR.Find("Date");
			_DatePanel.GetComponentAtPath("SelectedIndicatorText", out _SelectedDateText);
			int i = 0;
			_AllAdapters[i++] = DayAdapter = _DatePanel.GetComponentAtPath<DateTimePickerAdapter>("Panel/Day");
			_AllAdapters[i++] = MonthAdapter = _DatePanel.GetComponentAtPath<DateTimePickerAdapter>("Panel/Month");
			_AllAdapters[i++] = YearAdapter = _DatePanel.GetComponentAtPath<DateTimePickerAdapter>("Panel/Year");
			_TimePanel = adaptersTR.Find("Time");
			_TimePanel.GetComponentAtPath("SelectedIndicatorText", out _SelectedTimeText);
			_AllAdapters[i++] = HourAdapter = _TimePanel.GetComponentAtPath<DateTimePickerAdapter>("Panel/Hour");
			_AllAdapters[i++] = MinuteAdapter = _TimePanel.GetComponentAtPath<DateTimePickerAdapter>("Panel/Minute");
			_AllAdapters[i++] = SecondAdapter = _TimePanel.GetComponentAtPath<DateTimePickerAdapter>("Panel/Second");

			for (i = 0; i < _AllAdapters.Length; ++i)
				_AllAdapters[i].Parameters.UseUnscaledTime = _UseUnscaledTime;

			if (_AutoInit)
				ExecuteAfter(.2f, AutoInit);
		}

		void Update()
		{
			if (_Animating)
			{
				//transform.localScale = AnimCurrentFloat;
				_CanvasGroup.alpha = AnimCurrentFloat;

				if (AnimElapsedTime01 == 1f)
				{
					_Animating = false;
					if (_ActionOnAnimDone != null)
						_ActionOnAnimDone();
				}

				return;
			}

			if (!_Initialized)
				return;

			try
			{
				var current = SelectedValue;
				_SelectedDateText.text = _DisplaySelectedDateAsShort ? current.ToShortDateString() : current.ToLongDateString();
				_SelectedTimeText.text = _DisplaySelectedTimeAsShort ? current.ToShortTimeString() : current.ToLongTimeString();
			}
			catch /*(Exception e)*/{
				//Debug.Log(e + "\n"+YearAdapter.SelectedValue + "," + MonthAdapter.SelectedValue + "," + DayAdapter.SelectedValue + "," +
				//HourAdapter.SelectedValue + "," + MinuteAdapter.SelectedValue + "," + SecondAdapter.SelectedValue);
			}
		}

		public void InitWithNow() { InitWithDate(DateTime.Now); }

		public void InitWithDate(DateTime dateTime)
		{
			StopAnimations();
			_DateToInitWith = null;
			UnregisterAutoCorrection();

			int doneNum = 0;
			int targetDone = 2;
			Action onDone = () =>
			{
				if (++doneNum == targetDone)
				{
					_Initialized = true;
					RegisterAutoCorrection();
				}
			};

			//Func<float, bool> onProgress = p01 =>
			//{
			//	if (p01 == 1f)
			//	{
			//		if (++doneNum == targetDone)
			//		{
			//			_Initialized = true;
			//			RegisterAutoCorrection();
			//		}
			//	}
			//	return true;
			//};

			YearAdapter.ResetItems(3000);
			YearAdapter.SmoothScrollTo(dateTime.Year - 1, SCROLL_DURATION1, .5f, .5f);
			MonthAdapter.ResetItems(12);
			MonthAdapter.SmoothScrollTo(dateTime.Month - 1, SCROLL_DURATION2, .5f, .5f);
			DayAdapter.ResetItems(DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
			//DayAdapter.SmoothScrollTo(dateTime.Day - 1, SCROLL_DURATION3, .5f, .5f, onProgress, null, true);
			DayAdapter.SmoothScrollTo(dateTime.Day - 1, SCROLL_DURATION3, .5f, .5f, null, onDone, true);

			SecondAdapter.ResetItems(60);
			SecondAdapter.SmoothScrollTo(dateTime.Second, SCROLL_DURATION1, .5f, .5f);
			MinuteAdapter.ResetItems(60);
			MinuteAdapter.SmoothScrollTo(dateTime.Minute, SCROLL_DURATION2, .5f, .5f);
			HourAdapter.ResetItems(24);
			//HourAdapter.SmoothScrollTo(dateTime.Hour, SCROLL_DURATION3, .5f, .5f, onProgress, true);
			HourAdapter.SmoothScrollTo(dateTime.Hour, SCROLL_DURATION3, .5f, .5f, null, onDone, true);
		}

		public void ReturnCurrent()
		{
			//_AnimStart = Vector3.one;
			//_AnimEnd = Vector3.zero;
			_AnimStart = _CanvasGroup.alpha;
			_AnimEnd = 0f;
			_AnimStartTime = Time;
			_Animating = true;

			_ActionOnAnimDone = () =>
			{
				_ActionOnAnimDone = null;
				if (OnDateSelected != null)
					OnDateSelected(SelectedValue);

				Destroy(gameObject);
			};
		}

		void AutoInit()
		{
			_DateToInitWith = _DateToInitWith ?? DateTime.Now;
			InitWithDate(_DateToInitWith.Value);
		}

		void UnregisterAutoCorrection()
		{
			YearAdapter.OnSelectedValueChanged -= OnYearChanged;
			MonthAdapter.OnSelectedValueChanged -= OnMonthChanged;
		}

		void RegisterAutoCorrection()
		{
			YearAdapter.OnSelectedValueChanged += OnYearChanged;
			MonthAdapter.OnSelectedValueChanged += OnMonthChanged;
		}

		void OnYearChanged(int year) { OnMonthChanged(MonthAdapter.SelectedValue); }

		void OnMonthChanged(int month)
		{
			var selectedDay = DayAdapter.SelectedValue;
			int newDaysInMonth = DateTime.DaysInMonth(YearAdapter.SelectedValue, month);
			if (newDaysInMonth == DayAdapter.GetItemsCount())
				return;
			DayAdapter.ResetItems(newDaysInMonth);
			DayAdapter.ScrollTo(Math.Min(newDaysInMonth, selectedDay) - 1, .5f, .5f);
		}

		void StopAnimations()
		{
			foreach (var adapter in _AllAdapters)
			{
				if (adapter)
					adapter.CancelAllAnimations();
			}
		}


		void ExecuteAfter(float seconds, Action action) { StartCoroutine(ExecuteAfterCoroutine(seconds, action)); }
		IEnumerator ExecuteAfterCoroutine(float seconds, Action action)
		{
			if (seconds > 0f)
			{
				yield return null;
				yield return null;
			}

			if (_UseUnscaledTime)
				yield return new WaitForSecondsRealtime(seconds);
			else
				yield return new WaitForSeconds(seconds);

			action();
		}
	}
}
