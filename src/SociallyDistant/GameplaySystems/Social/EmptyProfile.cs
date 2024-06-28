#nullable enable
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Social;

namespace SociallyDistant.GameplaySystems.Social
{
	public class EmptyProfile : IProfile
	{
		/// <inheritdoc />
		public ObjectId ProfileId => ObjectId.Invalid;

		/// <inheritdoc />
		public CharacterAttributes Attributes => CharacterAttributes.None;

		/// <inheritdoc />
		public string SocialHandle { get; }

		/// <inheritdoc />
		public Gender Gender { get; }

		/// <inheritdoc />
		public string Bio { get; }

		/// <inheritdoc />
		public bool IsPrivate { get; }

		/// <inheritdoc />
		public string ChatName => "You";

		/// <inheritdoc />
		public string ChatUsername => "user";

		/// <inheritdoc />
		public Texture2D? Picture => null;

		/// <inheritdoc />
		public bool IsFriendsWith(IProfile friend)
		{
			if (friend == this)
				return true;

			return false;
		}

		/// <inheritdoc />
		public bool IsBlockedBy(IProfile user)
		{
			return false;
		}
	}
}