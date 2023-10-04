using Core.Serialization;
using UnityEngine.Analytics;

namespace Core.WorldData.Data
{
	public struct WorldProfileData :
		IWorldData,
		IDataWithId
	{
		private ObjectId id;
		private byte gender;
		private string avatarData;
		private string coverArtData;
		private bool isSocialPrivate;
		private string chatUsername;
		private string socialUsername;
		private string mailUsername;
		private string chatName;
		private string socialName;
		private string mailName;
		private string socialBio;

		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref gender, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref avatarData, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref coverArtData, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref isSocialPrivate, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref chatUsername, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref socialUsername, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref mailUsername, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref chatName, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref socialName, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref mailName, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref socialBio, serializer, WorldRevision.ChatAndSocialMedia, default);
		}

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public Gender Gender
		{
			get => (Gender) gender;
			set => gender = (byte) value;
		}

		public string AvatarData
		{
			get => avatarData;
			set => avatarData = value;
		}

		public string CoverArtData
		{
			get => coverArtData;
			set => coverArtData = value;
		}

		public bool IsSocialPrivate
		{
			get => isSocialPrivate;
			set => isSocialPrivate = value;
		}

		public string ChatUsername
		{
			get => chatUsername;
			set => chatUsername = value;
		}

		public string SocialUsername
		{
			get => socialUsername;
			set => socialUsername = value;
		}

		public string MailUsername
		{
			get => mailUsername;
			set => mailUsername = value;
		}

		public string ChatName
		{
			get => chatName;
			set => chatName = value;
		}

		public string SocialName
		{
			get => socialName;
			set => socialName = value;
		}

		public string MailName
		{
			get => mailName;
			set => mailName = value;
		}

		public string SocialBio
		{
			get => socialBio;
			set => socialBio = value;
		}
	}
}