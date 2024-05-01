using System;
using Core.Serialization;

namespace Core.WorldData.Data
{
	public struct DocumentElement : IWorldData
	{
		private byte elementType;
		private string data;
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			SerializationUtility.SerializeAtRevision(ref elementType, serializer, WorldRevision.Begin, default);
			SerializationUtility.SerializeAtRevision(ref data, serializer, WorldRevision.Begin, default);
		}

		public DocumentElementType ElementType
		{
			get => (DocumentElementType) elementType;
			set => elementType = (byte) value;
		}
		
		public string Data
		{
			get => data;
			set => data = value;
		}
	}
}