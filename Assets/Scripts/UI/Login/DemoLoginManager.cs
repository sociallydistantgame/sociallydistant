#nullable enable

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Login
{
	public class DemoLoginManager : 
		LoginManager,
		IPointerClickHandler,
		IUpdateSelectedHandler
	{
		private bool busy;
		
		private void Start()
		{
			EventSystem.current.SetSelectedGameObject(this.gameObject);
		}


		/// <inheritdoc />
		public void OnPointerClick(PointerEventData eventData)
		{
			if (busy)
				return;

			StartCoroutine(StartGame());
		}

		/// <inheritdoc />
		public void OnUpdateSelected(BaseEventData eventData)
		{
			if (busy)
				return;
			
			if (EventSystem.current == null)
				return;

			if (EventSystem.current.currentSelectedGameObject != this.gameObject)
				return;

			var ev = new Event();
			while (Event.PopEvent(ev))
			{
				if (ev.rawType == EventType.KeyDown)
				{
					StartCoroutine(StartGame());
				}
			}
		}

		private IEnumerator StartGame()
		{
			busy = true;
			
			// This is a coroutine because I intend on adding animations during polish.
			// That's a few months out so we'll just load the game immediately.
			yield return null;
		}
	}
}