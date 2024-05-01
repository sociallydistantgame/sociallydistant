using System;
using System.Collections.Generic;
using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldPostData :
		IWorldData,
		IDataWithId
	{
		private ObjectId id;
		private ObjectId author;
		private DateTime date;
		private IReadOnlyList<DocumentElement> documentElements;
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref author, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeAtRevision(ref date, serializer, WorldRevision.ChatAndSocialMedia, default);
			SerializationUtility.SerializeCollectionAtRevision(ref documentElements, serializer, WorldRevision.ChatAndSocialMedia);
		}

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public ObjectId Author
		{
			get => author;
			set => author = value;
		}

		public DateTime Date
		{
			get => date;
			set => date = value;
		}

		public IReadOnlyList<DocumentElement> DocumentElements
		{
			get => documentElements;
			set => documentElements = value;
		}
	}
}