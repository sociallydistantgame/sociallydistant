#nullable enable
using System;
using System.Collections.Generic;
using Social;
using UnityEngine;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public class ServerMembersController : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField]
		private ServerMembersListView listView = null!;

		public bool IsVisible
		{
			get => gameObject.activeSelf;
			set => gameObject.SetActive(value);
		}

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ServerMembersController));
		}

		public void UpdateMembers(IList<ServerMember> membersList)
		{
			this.listView.SetItems(membersList);
		}
	}
}