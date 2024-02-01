#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Core.Serialization;

namespace Core.WorldData
{
	public class WorldFlagCollection : IWorldFlagCollection
	{
		private readonly List<string> flags = new List<string>();
		
		/// <inheritdoc />
		public IEnumerator<string> GetEnumerator()
		{
			return flags.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(string item)
		{
			flags.Add(item);
		}

		/// <inheritdoc />
		public void Clear()
		{
			flags.Clear();
		}

		/// <inheritdoc />
		public bool Contains(string item)
		{
			return flags.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(string[] array, int arrayIndex)
		{
			flags.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(string item)
		{
			return flags.Remove(item);
		}

		/// <inheritdoc />
		public int Count => flags.Count;

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		public int IndexOf(string item)
		{
			return flags.IndexOf(item);
		}

		/// <inheritdoc />
		public void Insert(int index, string item)
		{
			flags.Insert(index, item);
		}

		/// <inheritdoc />
		public void RemoveAt(int index)
		{
			flags.RemoveAt(index);
		}

		/// <inheritdoc />
		public string this[int index]
		{
			get => flags[index];
			set => flags[index] = value;
		}

		/// <inheritdoc />
		public void Serialize(IWorldSerializer serializer)
		{
			if (serializer.IsReading)
			{
				this.Clear();
				var count = 0;

				SerializationUtility.SerializeAtRevision(ref count, serializer, WorldRevision.AddedWorldFlags, 0);

				for (var i = 0; i < count; i++)
				{
					string? value = null;
					
					SerializationUtility.SerializeAtRevision(ref value, serializer, WorldRevision.AddedWorldFlags, null);

					if (string.IsNullOrWhiteSpace(value))
						continue;
					
					Add(value);
				}
			}
			else
			{
				int count = this.Count;
				
				SerializationUtility.SerializeAtRevision(ref count, serializer, WorldRevision.AddedWorldFlags, 0);
				
				for (var i = 0; i < count; i++)
				{
					string? value = this[i];
					
					SerializationUtility.SerializeAtRevision(ref value, serializer, WorldRevision.AddedWorldFlags, null);
				}
			}
		}
	}
}