using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.Core.WorldData.Data
{
	[Serializable]
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

		public static bool operator ==(DocumentElement a, DocumentElement b)
		{
			return (a.ElementType == b.ElementType && a.Data == b.Data);
		}

		public static bool operator !=(DocumentElement a, DocumentElement b)
		{
			return !(a == b);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is DocumentElement element && element == this;
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return HashCode.Combine(ElementType, Data);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"(ElementType={ElementType}, Data={Data})";
		}
	}
}