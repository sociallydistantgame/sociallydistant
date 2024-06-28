using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	public struct WorldMailData :
		IWorldData,
		INarrativeObject,
		IDataWithId
	{
		private ObjectId id;
		private ObjectId from;
		private ObjectId to;
		private ObjectId threadId;
		private string subject;
		private IReadOnlyList<DocumentElement> document;
		private byte typeFlags;
		private string narrativeId;

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public ObjectId From
		{
			get => from;
			set => from = value;
		}

		public ObjectId To
		{
			get => to;
			set => to = value;
		}

		public ObjectId ThreadId
		{
			get => threadId;
			set => threadId = value;
		}

		public string Subject
		{
			get => subject;
			set => subject = value;
		}

		public IReadOnlyList<DocumentElement> Document
		{
			get => document;
			set => document = value;
		}

		public MailTypeFlags TypeFlags
		{
			get => (MailTypeFlags) this.typeFlags;
			set => this.typeFlags = (byte) value;
		}

		public string? NarrativeId
		{
			get => narrativeId;
			set => narrativeId = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.Email, default);
			SerializationUtility.SerializeAtRevision(ref from, serializer, WorldRevision.Email, default);
			SerializationUtility.SerializeAtRevision(ref to, serializer, WorldRevision.Email, default);
			SerializationUtility.SerializeAtRevision(ref threadId, serializer, WorldRevision.Email, default);
			SerializationUtility.SerializeAtRevision(ref subject, serializer, WorldRevision.Email, default);
			SerializationUtility.SerializeAtRevision(ref typeFlags, serializer, WorldRevision.MailTypes, default);
			SerializationUtility.SerializeAtRevision(ref narrativeId, serializer, WorldRevision.MailTypes, default);
			
			SerializationUtility.SerializeCollectionAtRevision(ref document, serializer, WorldRevision.Email);
		}
	}
}