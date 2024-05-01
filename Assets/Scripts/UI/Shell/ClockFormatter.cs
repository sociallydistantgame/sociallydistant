#nullable enable

using System;
using System.Text;
using Core;
using GamePlatform;
using TMPro;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace UI.Shell
{
	public class ClockFormatter : MonoBehaviour
	{
		[Header("Configuration")]
		[SerializeField]
		[Multiline]
		private string textFormat = string.Empty;

		[SerializeField]
		private bool abbreviateDayOfWeek;

		private TextMeshProUGUI text= null!;
		private IWorldManager worldHolder = null!;

		private void Awake()
		{
			worldHolder = GameManager.Instance.WorldManager;
			
			this.AssertAllFieldsAreSerialized(typeof(ClockFormatter));
			this.MustGetComponent(out text);
		}

		private void Update()
		{
			DateTime now = worldHolder.World.GlobalWorldState.Value.Now;

			string dayName = GetDayOfWeekText(now.DayOfWeek);
			string time = now.ToShortTimeString();

			SetClockText(dayName, time);
		}

		private void SetClockText(string dayName, string time)
		{
			string newText = textFormat.Replace("DAYNAME", dayName).Replace("TIME", time);

			if (this.text.text == newText)
				return;

			this.text.SetText(newText);
		}

		private string GetDayOfWeekText(DayOfWeek dayOfWeek)
		{
			string dayName = dayOfWeek switch
			{
				DayOfWeek.Friday => "Friday",
				DayOfWeek.Monday => "Monday",
				DayOfWeek.Saturday => "Saturday",
				DayOfWeek.Sunday => "Sunday",
				DayOfWeek.Thursday => "Thursday",
				DayOfWeek.Tuesday => "Tuesday",
				DayOfWeek.Wednesday => "wednesday",
				_ => "Doomsday" // There's no way we'll ever reach this.
			};

			return abbreviateDayOfWeek ? dayName.Substring(0, 3) : dayName;
		}
	}
}