using System;
using Core.Serialization;

namespace Core.DataManagement
{
	public class DataObject<TRevision, TDataElement> : 
		ISerializable<TRevision>,
		IWorldObject<TDataElement>
		where TRevision : Enum
		where TDataElement : struct, ISerializable<TRevision>
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

		public DataObject(DataEventDispatcher eventDispatcher)
		{
			this.eventDispatcher = eventDispatcher;
		}

		/// <inheritdoc />
		public void Serialize(IRevisionedSerializer<TRevision> serializer)
		{
			TDataElement oldValue = this.value;
			
			this.value.Serialize(serializer);

			if (serializer.IsReading)
				eventDispatcher.Modify.Invoke(oldValue, this.value);
		}
	}
}