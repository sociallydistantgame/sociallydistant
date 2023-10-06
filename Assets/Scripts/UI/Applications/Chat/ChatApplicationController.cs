#nullable enable

using System.Collections.Generic;
using GameplaySystems.Networld;
using GameplaySystems.Social;
using UnityEngine;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public class ChatApplicationController : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private SocialServiceHolder socialService = null!;
		
		[Header("UI")]
		[SerializeField]
		private ChatHeader chatHeader = null!;

		[SerializeField]
		private ServerMembersController serverMembers = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ChatApplicationController));
		}

		private void Start()
		{
			chatHeader.DisplayGuildHeader("#restitched-development", "A place to discuss development of Restitched.");
			serverMembers.IsVisible = true;

			var memberList = new List<ServerMember>
			{
				new ServerMember
				{
					DisplayName = "Trixel Creative",
					Username = "TrixelCreative"
				},
				new ServerMember
				{
					DisplayName = "acidic light",
					Username = "acidiclight"
				},
				new ServerMember
				{
					DisplayName = "Ritchie Frodomar",
					Username = "ritchie_246"
				}
			};
			
			serverMembers.UpdateMembers(memberList);
		}
	}
}