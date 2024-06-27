#nullable enable
using System;
using Chat;
using UnityEngine;
using UnityExtensions;

namespace GameplaySystems.Chat
{
	public sealed class ConversationBootstrap : MonoBehaviour
	{
		[SerializeField]
		private ConversationManager prefab = null!;

		private ConversationManager conversationManager;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ConversationBootstrap));
			conversationManager = Instantiate(prefab);
		}
	}
}