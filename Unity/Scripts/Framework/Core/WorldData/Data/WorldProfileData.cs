using Core.Serialization;
using UnityEngine.Analytics;

namespace Core.WorldData.Data
{
	public struct WorldProfileData :
		IWorldData,
		IDataWithId,
		INarrativeObject
	{
		private ObjectId id;
		private string narrativeId;
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
		private ushort attributes;

		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			string ignored = string.Empty;
			
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref gender, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref avatarData, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref coverArtData, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref isSocialPrivate, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref chatUsername, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref ignored, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref ignored, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref chatName, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref ignored, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref ignored, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref socialBio, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref narrativeId, serializer, WorldRevision.NarrativeProfiles, default);
			SerializationUtility.SerializeAtRevision(ref attributes, serializer, WorldRevision.CharacterAttributes, default);
		}

		public CharacterAttributes Attributes
		{
			get => (CharacterAttributes) attributes;
			set => attributes = (ushort) value;
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
		
		public string ChatName
		{
			get => chatName;
			set => chatName = value;
		}

		public string SocialBio
		{
			get => socialBio;
			set => socialBio = value;
		}

		public string NarrativeId
		{
			get => narrativeId;
			set => narrativeId = value;
		}
	}
}