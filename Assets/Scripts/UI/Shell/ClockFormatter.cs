#nullable enable

using System;
using Core;
using TMPro;
using UnityEngine;
using Utility;

namespace UI.Shell
{
	public class ClockFormatter : MonoBehaviour
	{
		private TextMeshProUGUI text= null!;

		[Header("Dependencies")]
		[SerializeField]
		private WorldManagerHolder worldHolder = null!;

		[Header("Configuration")]
		[SerializeField]
		[Multiline]
		private string textFormat = string.Empty;

		[SerializeField]
		private bool abbreviateDayOfWeek;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ClockFormatter));
			this.MustGetComponent(out text);
		}

		private void Update()
		{
			DateTime now = worldHolder.Value.World.GlobalWorldState.Value.Now;

			string dayName = GetDayOfWeekText(now.DayOfWeek);
			string time = now.ToShortTimeString();

			SetClockText(dayName, time);
		}

		private void SetClockText(string dayName, string time)
		{
			text.SetText(textFormat.Replace("DAYNAME", dayName).Replace("TIME", time));
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