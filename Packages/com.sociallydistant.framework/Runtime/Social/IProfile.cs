using Core;
using UnityEngine;
using UnityEngine.Analytics;

namespace Social
{
	public interface IProfile
	{
		ObjectId ProfileId { get; }
		
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