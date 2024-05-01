#nullable enable
using System;
using TMPro;
using UI.Applications.Chat;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace GameplaySystems.Chat
{
	public sealed class ChatEntryFocusHandler : 
		UIBehaviour,
		ISelectHandler,
		IDeselectHandler
	{
		[SerializeField]
		private ConversationBranchList branchList = null!;
		
		private TMP_InputField inputField = null!;
		private IDisposable? queryObserver;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ChatEntryFocusHandler));
			this.MustGetComponentInChildren(out inputField);
			
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			queryObserver = inputField.onValueChanged.AsObservable()
				.Delay(TimeSpan.FromSeconds(0.1f))
				.Subscribe(UpdateQuery);
		}

		/// <inheritdoc />
		protected override void OnDestroy()
		{
			queryObserver?.Dispose();
			base.OnDestroy();
		}

		/// <inheritdoc />
		public void OnSelect(BaseEventData eventData)
		{
			if (branchList.IsEmpty)
				return;

			branchList.Show();
		}

		/// <inheritdoc />
		public void OnDeselect(BaseEventData eventData)
		{
			branchList.Hide();
		}

		private void UpdateQuery(string newQuery)
		{
			this.branchList.UpdateQuery(newQuery);
		}
	}
}