#nullable enable
using Core;
using Social;
using UnityEngine.Analytics;

namespace GameplaySystems.Social
{
	public class EmptyProfile : IProfile
	{
		/// <inheritdoc />
		public ObjectId ProfileId => ObjectId.Invalid;

		/// <inheritdoc />
		public Gender Gender { get; }

		/// <inheritdoc />
		public string Bio { get; }

		/// <inheritdoc />
		public bool IsPrivate { get; }
	}
}