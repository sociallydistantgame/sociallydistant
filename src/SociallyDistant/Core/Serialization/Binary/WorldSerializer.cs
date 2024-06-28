#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Serialization;
using SociallyDistant.Core.WorldData;

namespace SociallyDistant.Core.Serialization.Binary
{
	public class WorldSerializer : 
		IWorldSerializer
	{
		private WorldRevisionComparer revisionComparer;
		private IDataWriter? writer;
		private IDataReader? reader;

		/// <inheritdoc />
		public IRevisionComparer<WorldRevision> RevisionComparer => revisionComparer;

		/// <inheritdoc />
		public bool IsWriting => writer != null;

		/// <inheritdoc />
		public bool IsReading => reader != null;

		public WorldSerializer(IDataReader reader)
		{
			this.writer = null;
			this.reader = reader;

			WorldRevision revision = (WorldRevision) reader.Read_short();
			
			// Throw if the save revision is higher than our latest revision, because it means
			// that it was saved by a newer build of the game.
			if (revision >= WorldRevision.Latest)
				throw new InvalidOperationException($"Cannot load the given World data because it was saved by a newer build of Socially Distant. (Current world revision: {WorldRevision.Latest - 1}, save file revision: {revision})");

			this.revisionComparer = new WorldRevisionComparer(revision);
		}

		public WorldSerializer(IDataWriter writer)
		{
			this.reader = null;
			this.writer = writer;
			this.revisionComparer = new WorldRevisionComparer(); // will automatically set current revision to the latest one
			
			// It's up to us to write the revision.
			var rawRevision = (short) revisionComparer.Current;
			writer.Write(rawRevision);
		}

		/// <inheritdoc />
		public void Serialize(ref DateTime value)
		{
			if (IsReading)
				value = reader.ReadDateTime();
			else
				writer.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref sbyte value)
		{
			if (IsReading)
				value = reader!.Read_sbyte();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref byte value)
		{
			if (IsReading)
				value = reader!.Read_byte();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref short value)
		{
			if (IsReading)
				value = reader!.Read_short();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref ushort value)
		{
			if (IsReading)
				value = reader!.Read_ushort();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref int value)
		{
			if (IsReading)
				value = reader!.Read_int();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref uint value)
		{
			if (IsReading)
				value = reader!.Read_uint();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref long value)
		{
			if (IsReading)
				value = reader!.Read_long();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref ulong value)
		{
			if (IsReading)
				value = reader!.Read_ulong();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref bool value)
		{
			if (IsReading)
				value = reader!.Read_bool();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref float value)
		{
			if (IsReading)
				value = reader!.Read_float();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref double value)
		{
			if (IsReading)
				value = reader!.Read_double();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref decimal value)
		{
			if (IsReading)
				value = reader!.Read_decimal();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref char value)
		{
			if (IsReading)
				value = reader!.Read_char();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref string value)
		{
			if (IsReading)
				value = reader!.Read_string();
			else if (IsWriting)
				writer!.Write(value);
		}

		/// <inheritdoc />
		public void Serialize(ref ISerializable value)
		{
			if (IsReading)
				value.Read(reader!);
			else if (IsWriting)
				value.Write(writer!);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			reader?.Dispose();
			writer?.Dispose();
		}
	}
}