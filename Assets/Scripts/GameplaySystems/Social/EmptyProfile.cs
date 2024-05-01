#nullable enable
using Core;
using Social;
using UnityEngine;
using UnityEngine.Analytics;

namespace GameplaySystems.Social
{
	public class EmptyProfile : IProfile
	{
		/// <inheritdoc />
		public ObjectId ProfileId => ObjectId.Invalid;

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