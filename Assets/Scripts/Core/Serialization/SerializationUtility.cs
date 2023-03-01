using System;

namespace Core.Serialization
{
	public static class SerializationUtility
	{
		public static void SerializeAtRevision<TRevision>(ref sbyte value, IRevisionedSerializer<TRevision> serializer,  TRevision revision, sbyte defaultValue)
			where TRevision : Enum
		{
			if (serializer.IsReading)
			{
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
				if (serializer.RevisionComparer.IsOlder(revision))
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
	}
}