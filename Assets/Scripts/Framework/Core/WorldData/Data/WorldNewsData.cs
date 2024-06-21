using System;
using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct WorldNewsData : 
		IWorldData,
		INarrativeObject,
		IDataWithId
	{
		private ObjectId instanceId;
		private string narrativeId;
		private DateTime date;

		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref instanceId, serializer, WorldRevision.Articles, default);
			SerializationUtility.SerializeAtRevision(ref narrativeId, serializer, WorldRevision.Articles, default);
			SerializationUtility.SerializeAtRevision(ref date, serializer, WorldRevision.Articles, default);
		}

		/// <inheritdoc />
		public string NarrativeId
		{
			get => narrativeId;
			set => narrativeId = value;
		}

		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => instanceId;
			set => instanceId = value;
		}

		public DateTime Date
		{
			get => date;
			set => date = value;
		}
	}
}