using System;
using System.Collections;
using System.Collections.Generic;
using Core.Serialization;
using Core.Systems;

namespace Core.DataManagement
{
	public sealed class NarrativeObjectTable<TDataElement> : INarrativeObjectTable<TDataElement>
		where TDataElement : struct, IWorldData, IDataWithId, INarrativeObject
	{
		private readonly UniqueIntGenerator idGenerator;
		private readonly WorldDataTable<TDataElement> underlyingTable;
		private readonly Dictionary<string, ObjectId> narrativeIdLookup = new();
		private readonly Dictionary<ObjectId, string> objectIdLookup = new();
		private readonly bool createAutomatically;

		public NarrativeObjectTable(UniqueIntGenerator idGenerator, DataEventDispatcher eventDispatcher, bool createImmediately = true)
		{
			this.createAutomatically = createImmediately;
			this.idGenerator = idGenerator;
			underlyingTable = new WorldDataTable<TDataElement>(idGenerator, eventDispatcher, true);
		}

		public void Clear()
		{
			narrativeIdLookup.Clear();
			objectIdLookup.Clear();
			underlyingTable.Clear();
		}
		
		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer, WorldRevision revision)
		{
			underlyingTable.Serialize(serializer, revision);

			if (serializer.IsReading)
			{
				narrativeIdLookup.Clear();
				objectIdLookup.Clear();
				
				foreach (TDataElement element in underlyingTable)
				{
					if (string.IsNullOrWhiteSpace(element.NarrativeId))
						continue;

					narrativeIdLookup[element.NarrativeId] = element.InstanceId;
					objectIdLookup[element.InstanceId] = element.NarrativeId;
				}
			}
		}

		/// <inheritdoc />
		public TDataElement GetNarrativeObject(string narrativeId)
		{
			if (!narrativeIdLookup.TryGetValue(narrativeId, out ObjectId existingElementId))
			{
				var element = new TDataElement();
				element.InstanceId = idGenerator.GetNextValue();
				element.NarrativeId = narrativeId;

				if (createAutomatically)
					Add(element);
				else
					return element;
			}

			return this[narrativeIdLookup[narrativeId]];
		}

		/// <inheritdoc />
		public bool ContainsNarrativeId(string narrativeId)
		{
			return this.narrativeIdLookup.ContainsKey(narrativeId);
		}

		/// <inheritdoc />
		public IEnumerator<TDataElement> GetEnumerator()
		{
			return underlyingTable.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public TDataElement this[ObjectId id] => underlyingTable[id];

		/// <inheritdoc />
		public void Add(TDataElement data)
		{
			if (string.IsNullOrWhiteSpace(data.NarrativeId))
			{
				underlyingTable.Add(data);
				return;
			}

			if (narrativeIdLookup.ContainsKey(data.NarrativeId))
				throw new InvalidOperationException($"Duplicate Narrative ID assigned to existing object {narrativeIdLookup[data.NarrativeId]} and new object {data.InstanceId}");

			narrativeIdLookup[data.NarrativeId] = data.InstanceId;
			objectIdLookup[data.InstanceId] = data.NarrativeId;
			
			underlyingTable.Add(data);
		}

		/// <inheritdoc />
		public void Remove(TDataElement data)
		{
			underlyingTable.Remove(data);

			if (!objectIdLookup.TryGetValue(data.InstanceId, out string narrativeId))
				return;

			objectIdLookup.Remove(data.InstanceId);
			narrativeIdLookup.Remove(narrativeId);
		}

		/// <inheritdoc />
		public void Modify(TDataElement data)
		{
			if (objectIdLookup.TryGetValue(data.InstanceId, out string oldNarrativeId))
			{
				narrativeIdLookup.Remove(oldNarrativeId);
				
				if (string.IsNullOrWhiteSpace(data.NarrativeId))
				{
					objectIdLookup.Remove(data.InstanceId);
				}
				else
				{
					objectIdLookup[data.InstanceId] = data.NarrativeId;
					narrativeIdLookup[data.NarrativeId] = data.InstanceId;
				}
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(data.NarrativeId))
				{
					if (narrativeIdLookup.ContainsKey(data.NarrativeId))
						throw new InvalidOperationException($"Duplicate Narrative ID assigned to existing object {narrativeIdLookup[data.NarrativeId]} and new object {data.InstanceId}");

					narrativeIdLookup[data.NarrativeId] = data.InstanceId;
					objectIdLookup[data.InstanceId] = data.NarrativeId;
				}
			}
			
			underlyingTable.Modify(data);
		}

		/// <inheritdoc />
		public TDataElement[] ToArray()
		{
			return underlyingTable.ToArray();
		}

		/// <inheritdoc />
		public bool ContainsId(ObjectId id)
		{
			return underlyingTable.ContainsId(id);
		}
	}
}