#nullable enable
using Core.DataManagement;
using Core.Serialization;
using GameplaySystems.Networld;
using Networking;

namespace Core.WorldData.Data
{
	public struct WorldHackableData : 
		IWorldData,
		IDataWithId
	{
		private ObjectId id;
		private ObjectId computerId;
		private ushort port;
		private byte serverType;
		private byte secLevel;
		
		
		/// <inheritdoc />
		public ObjectId InstanceId
		{
			get => id;
			set => id = value;
		}

		public ObjectId ComputerId
		{
			get => computerId;
			set => computerId = value;
		}

		public ushort Port
		{
			get => port;
			set => port = value;
		}

		public ServerType ServerType
		{
			get => (ServerType) serverType;
			set => serverType = (byte) value;
		}

		public SecurityLevel SecurityLevel
		{
			get => (SecurityLevel) secLevel;
			set => secLevel = (byte) value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.AddedHackables, default);
			SerializationUtility.SerializeAtRevision(ref computerId, serializer, WorldRevision.AddedHackables, default);
			SerializationUtility.SerializeAtRevision(ref port, serializer, WorldRevision.AddedHackables, default);
			SerializationUtility.SerializeAtRevision(ref serverType, serializer, WorldRevision.AddedHackables, default);
			SerializationUtility.SerializeAtRevision(ref secLevel, serializer, WorldRevision.AddedHackables, default);
		}
	}
}