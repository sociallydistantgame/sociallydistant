#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using Utility;

namespace UI.Shell.InfoPanel
{
	public class InfoPanelController : MonoBehaviour
	{
		private readonly List<InfoWidgetData> widgetList = new List<InfoWidgetData>();
		
		[SerializeField]
		private InfoWidgetsController widgetsArea = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(InfoPanelController));
		}

		private IEnumerator Start()
		{
			yield return null;
			this.widgetsArea.SetItems(this.widgetList);
		}
	}
}