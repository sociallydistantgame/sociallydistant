using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Input
{
	public static class InputValidators
	{
		public delegate char StringValidator(string text, int charIndex, char addedChar);
	}
}