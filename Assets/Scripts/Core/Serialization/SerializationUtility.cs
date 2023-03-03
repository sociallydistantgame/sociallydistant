using System;
using System.Collections.Generic;

namespace Core.Serialization
{
	public static class SerializationUtility
	{
		public static void SerializeAtRevision<TRevision>(ref sbyte value, IRevisionedSerializer<TRevision> serializer,  TRevision revision, sbyte defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref byte value, IRevisionedSerializer<TRevision> serializer, TRevision revision, byte defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref short value, IRevisionedSerializer<TRevision> serializer,  TRevision revision, short defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref ushort value, IRevisionedSerializer<TRevision> serializer, TRevision revision, ushort defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref int value, IRevisionedSerializer<TRevision> serializer, TRevision revision, int defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref uint value, IRevisionedSerializer<TRevision> serializer, TRevision revision, uint defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref long value, IRevisionedSerializer<TRevision> serializer, TRevision revision, long defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref ulong value, IRevisionedSerializer<TRevision> serializer,  TRevision revision, ulong defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref bool value, IRevisionedSerializer<TRevision> serializer, TRevision revision, bool defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref float value, IRevisionedSerializer<TRevision> serializer,  TRevision revision, float defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref double value, IRevisionedSerializer<TRevision> serializer, TRevision revision, double defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref decimal value, IRevisionedSerializer<TRevision> serializer, TRevision revision, decimal defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref char value, IRevisionedSerializer<TRevision> serializer, TRevision revision, char defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision>(ref string value, IRevisionedSerializer<TRevision> serializer, TRevision revision, string defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else serializer.Serialize(ref value);
			}
			else if (serializer.IsWriting)
			{
				serializer.Serialize(ref value);
			}
		}

		public static void SerializeAtRevision<TRevision, TSerializable>(ref TSerializable value, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
			where TSerializable : struct, ISerializable<TRevision>
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsCurrentOrNewer(revision))
					value.Serialize(serializer);
			}
			else
				value.Serialize(serializer);
		}

		public static void SerializeAtRevision<TRevision, TSerializable>(ref TSerializable value, IRevisionedSerializer<TRevision> serializer, TRevision revision, TSerializable defaultValue)
			where TRevision : Enum
			where TSerializable : struct, ISerializable
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsNewer(revision))
					value = defaultValue;
				else
				{
					ISerializable s = value;
					serializer.Serialize(ref s);
					value = (TSerializable) s;
				}
			}
			else
			{
				ISerializable s = value;
				serializer.Serialize(ref s);
				value = (TSerializable) s;
			}
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out sbyte value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out byte value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out short value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out ushort value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out int value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out uint value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out long value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out ulong value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out bool value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out float value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out double value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out decimal value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out char value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out string value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}
		
		public static bool IgnoreAfterRevision<TRevision>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out ISerializable value)
			where TRevision : Enum
		{
			value = default;
			
			if (serializer.IsWriting)
				return false;
				
			if (!serializer.IsReading)
				return false;
				
			// Ignore if the provided revision is older than or the same as
			// the current revision.
			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			serializer.Serialize(ref value);
			return true;
		}

		public static bool IgnoreAfterRevision<TRevision, TSerializable>(IRevisionedSerializer<TRevision> serializer, TRevision revision, out TSerializable value)
			where TRevision : Enum
			where TSerializable : struct, ISerializable<TRevision>
		{
			value = default;
			if (serializer.IsWriting)
				return false;

			if (!serializer.IsReading)
				return false;


			if (serializer.RevisionComparer.IsCurrentOrOlder(revision))
				return false;
				
			value.Serialize(serializer);
			return true;
		}

		#region Collection serialization

		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<sbyte> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<sbyte>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					sbyte newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					sbyte element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<byte> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<byte>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					byte newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					byte element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<short> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<short>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					short newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					short element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<ushort> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<ushort>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					ushort newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					ushort element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<int> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<int>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					int newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					int element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<uint> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<uint>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					uint newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					uint element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<long> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<long>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					long newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					long element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<ulong> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<ulong>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					ulong newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					ulong element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<bool> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<bool>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					bool newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					bool element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<float> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<float>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					float newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					float element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<double> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<double>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					double newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					double element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<decimal> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<decimal>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					decimal newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					decimal element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<char> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<char>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					char newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					char element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TRevision>(ref IReadOnlyList<string> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<string>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					string newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					string element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		
		public static void SerializeCollectionAtRevision<TSerializable, TRevision>(ref IReadOnlyList<TSerializable> collection, IRevisionedSerializer<TRevision> serializer, TRevision revision)
			where TRevision : Enum
			where TSerializable : struct, ISerializable
		{
			if (serializer.IsReading)
			{
				var newCollection = new List<TSerializable>();
				var newCount = 0;
				SerializeAtRevision(ref newCount, serializer, revision, 0);
				
				for (var i = 0; i < newCount; i++)
				{
					TSerializable newElement = default;
					SerializeAtRevision(ref newElement, serializer, revision, default);
					newCollection.Add(newElement);
				}
				
				collection = newCollection;
			}
			
			if (serializer.IsWriting)
			{
				int elementCount = collection.Count;
				SerializeAtRevision(ref elementCount, serializer, revision, elementCount);
				
				for (var i = 0; i < elementCount; i++)
				{
					TSerializable element = collection[i];
					SerializeAtRevision(ref element, serializer, revision, element);
				}
			}
		}
		

		#endregion
	}
}