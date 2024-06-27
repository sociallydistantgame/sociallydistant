using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Serialization;
using Core.Systems;
using UnityEngine.Assertions;

namespace Core.DataManagement
{
	public class DataTable<TDataElement, TRevision, TSerializer> :
		ISerializableDataTable<TDataElement, TRevision, TSerializer>
		where TSerializer : IRevisionedSerializer<TRevision>
		where TDataElement : struct, ISerializable<TRevision, TSerializer>, IDataWithId
		where TRevision : Enum
	{
		private readonly bool dispatchOnSerialize;
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

		protected DataTable(UniqueIntGenerator instanceIdgenerator, DataEventDispatcher eventDispatcher, bool dispatchOnSerialize)
		{
			this.instanceIdGenerator = instanceIdgenerator;
			this.eventDispatcher = eventDispatcher;
			this.dispatchOnSerialize = dispatchOnSerialize;
		}

		public void Add(TDataElement data)
		{
			AddInternal(data, true);
		}

		private void AddInternal(TDataElement data, bool dispatch)
		{
			if (data.InstanceId.IsInvalid)
				throw new InvalidOperationException("Cannot add data with an invalid instance ID to a data table.");
			
			if (dataIndexMap.ContainsKey(data.InstanceId))
				throw new InvalidOperationException($"Duplicate instance ID: {data.InstanceId.Id}");

			dataIndexMap.Add(data.InstanceId, dataElements.Count);
			dataElements.Add(data);
			
			if (dispatch)
				eventDispatcher.Create.Invoke(data);
		}

		public void Remove(TDataElement data)
		{
			if (!dataIndexMap.TryGetValue(data.InstanceId, out int index))
				throw new InvalidOperationException($"Instance ID not found: {data.InstanceId.Id}");

			instanceIdGenerator.DeclareUnused(data.InstanceId.Id);
            
			dataElements.RemoveAt(index);
			dataIndexMap.Remove(data.InstanceId);
			eventDispatcher.Delete.Invoke(data);

			ShiftIndices(index);
		}

		private void ShiftIndices(int index)
		{
			foreach (ObjectId objectId in this.dataIndexMap.Keys.ToArray())
			{
				if (dataIndexMap[objectId] > index)
					dataIndexMap[objectId]--;
			}
		}

		public void Modify(TDataElement data)
		{
			if (!dataIndexMap.TryGetValue(data.InstanceId, out int index))
				throw new InvalidOperationException($"Instance ID not found: {data.InstanceId.Id}");

			TDataElement previous = dataElements[index];
			dataElements[index] = data;
			eventDispatcher.Modify.Invoke(previous, data);
		}

		public void Clear()
		{
			while (dataElements.Count > 0)
			{
				TDataElement element = dataElements[0];
				Remove(element);
			}
			
			Assert.IsFalse(dataElements.Count > 0);
			Assert.IsFalse(this.dataIndexMap.Count > 0);
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

				AddInternal(element, dispatchOnSerialize);
			}
		}

		public TDataElement[] ToArray()
		{
			return this.dataElements.ToArray();
		}

		/// <inheritdoc />
		public bool ContainsId(ObjectId id)
		{
			return dataIndexMap.ContainsKey(id);
		}

		/// <inheritdoc />
		public IEnumerator<TDataElement> GetEnumerator()
		{
			return this.dataElements.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}