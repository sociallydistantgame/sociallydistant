#nullable enable
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Social;

namespace SociallyDistant.GameplaySystems.Social
{
	public class Profile : IProfile
	{
		private readonly SocialService socialService;
		private WorldProfileData data;

		public Profile(SocialService socialService)
		{
			this.socialService = socialService;
		}

		internal void SetData(WorldProfileData newData)
		{
			this.data = newData;
		}

		/// <inheritdoc />
		public ObjectId ProfileId => data.InstanceId;

		/// <inheritdoc />
		public CharacterAttributes Attributes => data.Attributes;

		/// <inheritdoc />
		public string SocialHandle => data.ChatUsername;

		/// <inheritdoc />
		public Gender Gender => data.Gender;

		/// <inheritdoc />
		public string Bio => data.SocialBio;

		/// <inheritdoc />
		public bool IsPrivate => data.IsSocialPrivate;

		/// <inheritdoc />
		public string ChatName => data.ChatName;

		/// <inheritdoc />
		public string ChatUsername => data.ChatUsername;

		/// <inheritdoc />
		public Texture2D? Picture => null;

		/// <inheritdoc />
		public bool IsFriendsWith(IProfile friend)
		{
			if (friend == this)
				return true;

			return socialService.GetFriends(this).Contains(friend);
		}

		/// <inheritdoc />
		public bool IsBlockedBy(IProfile user)
		{
			if (user == this)
				return false;

			return socialService.GetBlockedProfiles(user).Contains(this);
		}
	}
}