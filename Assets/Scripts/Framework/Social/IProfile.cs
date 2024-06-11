using Core;
using Core.WorldData.Data;
using UnityEngine;
using UnityEngine.Analytics;

namespace Social
{
	public interface IProfile
	{
		ObjectId ProfileId { get; }
		
		CharacterAttributes Attributes { get; }
		
		string SocialHandle { get; }
		
		Gender Gender { get; }
		string Bio { get; }
		
		bool IsPrivate { get; }
		
		string ChatName { get; }
		string ChatUsername { get; }
		Texture2D? Picture { get; }

		bool IsFriendsWith(IProfile friend);
		bool IsBlockedBy(IProfile user);
	}
}