#nullable enable

using Core.Serialization;

namespace Core.DataManagement
{
	public struct ObjectId : ISerializable
	{
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
	}
}