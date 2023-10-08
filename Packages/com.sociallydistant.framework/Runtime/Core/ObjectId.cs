#nullable enable

using Core.Serialization;

namespace Core
{
	public struct ObjectId : ISerializable
	{
		public bool Equals(ObjectId other)
		{
			return Id == other.Id;
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return obj is ObjectId other && Equals(other);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return Id;
		}

		public int Id;

		public bool IsInvalid => Id == -1;

		public ObjectId(int id)
		{
			this.Id = id;
		}
		
		/// <inheritdoc />
		public void Write(IDataWriter writer)
		{
			writer.Write(Id);
		}

		/// <inheritdoc />
		public void Read(IDataReader reader)
		{
			Id = reader.Read_int();
		}

		public static implicit operator ObjectId(int value)
		{
			return new ObjectId(value);
		}
		
		public static readonly ObjectId Invalid = -1;

		public static bool operator ==(ObjectId left, ObjectId right)
			=> left.Id == right.Id;

		public static bool operator !=(ObjectId left, ObjectId right)
			=> !(left == right);

		/// <inheritdoc />
		public override string ToString()
		{
			if (IsInvalid)
				return "Invalid";

			return Id.ToString();
		}
	}
}