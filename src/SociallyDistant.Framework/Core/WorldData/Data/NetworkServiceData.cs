#nullable enable
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	public struct NetworkServiceData : IWorldData
	{
		private string id;
		private bool enabled;
		private ushort port;

		public string Id
		{
			get => id;
			set => id = value;
		}

		public bool Enabled
		{
			get => enabled;
			set => enabled = value;
		}

		public ushort Port
		{
			get => port;
			set => port = value;
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref id, serializer, WorldRevision.ComputersCanListen, string.Empty);
			SerializationUtility.SerializeAtRevision(ref enabled, serializer, WorldRevision.ComputersCanListen, false);
			SerializationUtility.SerializeAtRevision(ref port, serializer, WorldRevision.ComputersCanListen, 0);
		}
	}
}