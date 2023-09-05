using System;
using System.Collections.Generic;
using Core.Serialization;
using Core.Systems;

namespace Core.DataManagement
{
	public class DataTable<TDataElement, TRevision, TSerializer> :
		ISerializableDataTable<TDataElement, TRevision, TSerializer>
		where TSerializer : IRevisionedSerializer<TRevision>
		where TDataElement : struct, ISerializable<TRevision, TSerializer>, IDataWithId
		where TRevision : Enum
	{
		private UniqueIntGenerator instanceIdGenerator;
		private List<TDataElement> dataElements = new List<TDataElement>();
		private Dictionary<ObjectId, int> dataIndexMap = new Dictionary<ObjectId, int>();
		private readonly DataEventDispatcher eventDispatcher;

		public TDataElement this[ObjectId instanceId]
		{
			get
			{
				if (!dataIndexMap.TryGetValue(instanceId, out int index))
					throw new IndexOutOfRangeException($"Instance ID {instanceId.Id} was not found in the data table.");

				return dataElements[index];
			}
		}

		protected DataTable(UniqueIntGenerator instanceIdgenerator, DataEventDispatcher eventDispatcher)
		{
			this.instanceIdGenerator = instanceIdgenerator;
			this.eventDispatcher = eventDispatcher;
		}

		public void Add(TDataElement data)
		{
			if (data.InstanceId.IsInvalid)
				throw new InvalidOperationException("Cannot add data with an invalid instance ID to a data table.");
			
			if (dataIndexMap.ContainsKey(data.InstanceId))
				throw new InvalidOperationException($"Duplicate instance ID: {data.InstanceId.Id}");

			dataIndexMap.Add(data.InstanceId, dataElements.Count);
			dataElements.Add(data);
			
			eventDispatcher.Create.Invoke(data);
		}

		public void Remove(TDataElement data)
		{
			if (!dataIndexMap.TryGetValue(data.InstanceId, out int index))
				throw new InvalidOperationException($"Instance ID not found: {data.InstanceId.Id}");

			dataElements.RemoveAt(index);
			dataIndexMap.Remove(data.InstanceId);
			eventDispatcher.Delete.Invoke(data);
		}

		public void Modify(TDataElement data)
		{
			if (!dataIndexMap.TryGetValue(data.InstanceId, out int index))
				throw new InvalidOperationException($"Instance ID not found: {data.InstanceId.Id}");

			TDataElement previous = dataElements[index];
			dataElements[index] = data;
			instanceIdGenerator.DeclareUnused(data.InstanceId.Id);
			eventDispatcher.Modify.Invoke(previous, data);
		}

		public void Clear()
		{
			while (dataElements.Count > 0)
			{
				TDataElement element = dataElements[0];
				Remove(element);
			}
		}
		
		public void Serialize(TSerializer serializer, TRevision revision)
		{
			// How many elements do we have in the data table?
			int dataCount = dataElements.Count;
			SerializationUtility.SerializeAtRevision(ref dataCount, serializer, revision, 0);

			if (serializer.IsReading)
			{
				dataElements.Clear();
				dataIndexMap.Clear();

				Read(serializer, dataCount);
			}
			else if (serializer.IsWriting)
			{
				Write(serializer, dataCount);
			}
		}

		private void Write(TSerializer serializer, int dataCount)
		{
			for (var i = 0; i < dataCount; i++)
			{
				TDataElement element = dataElements[i];
				SerializationUtility.SerializeAtRevision(ref element, serializer, serializer.RevisionComparer.Earliest);
			}
		}
		
		private void Read(TSerializer serializer, int dataCount)
		{
			for (var i = 0; i < dataCount; i++)
			{
				TDataElement element = new TDataElement();
				SerializationUtility.SerializeAtRevision(ref element, serializer, serializer.RevisionComparer.Earliest);

				instanceIdGenerator.ClaimUsedValue(element.InstanceId.Id);

				this.dataIndexMap.Add(element.InstanceId, i);
				
				dataElements.Add(element);
				
				// Fire the Create event for this data element.
				this.eventDispatcher.Create.Invoke(element);
			}
		}

		public TDataElement[] ToArray()
		{
			return this.dataElements.ToArray();
		}
	}
}