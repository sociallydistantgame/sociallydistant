using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Serialization;

namespace SociallyDistant.Core.DataManagement
{
	public abstract class DataObject<TRevision, TDataElement, TSerializer> : 
		ISerializable<TRevision, TSerializer>,
		ISerializableDataObject<TDataElement, TRevision, TSerializer>
		where TRevision : Enum
		where TDataElement : struct, ISerializable<TRevision, TSerializer>
		where TSerializer : IRevisionedSerializer<TRevision>
	{
		private readonly DataEventDispatcher eventDispatcher;
		private TDataElement value;

		public TDataElement Value
		{
			get => value;
			set
			{
				TDataElement oldValue = this.Value;
				this.value = value;
				this.eventDispatcher.Modify.Invoke(oldValue, value);
			}
		}

		protected DataObject(DataEventDispatcher eventDispatcher)
		{
			this.eventDispatcher = eventDispatcher;
		}

		/// <inheritdoc />
		public void Serialize(TSerializer serializer)
		{
			TDataElement oldValue = this.value;
			
			this.value.Serialize(serializer);

			if (serializer.IsReading)
				eventDispatcher.Modify.Invoke(oldValue, this.value);
		}
	}
}