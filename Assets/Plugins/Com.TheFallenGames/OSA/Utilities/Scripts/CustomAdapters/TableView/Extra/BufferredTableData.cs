using System;
using System.Collections;
using Com.TheFallenGames.OSA.DataHelpers;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Extra
{
	/// <summary>
	/// A table data that's not fetched all at once, but rather on demand, in chunks.
	/// </summary>
	public class BufferredTableData : TableData
	{
		/// <summary>
		/// </summary>
		/// <param name="into">Array to read data into, starting at 0, and ending at countToRead-1</param>
		/// <param name="firstItemIndex">This is not the index at which to insert the item, but rather the index of the item in your table</param>
		public delegate void TuplesChunkReader(ITuple[] into, int firstItemIndex, int countToRead);

		public override int Count { get { return _DataSource.Count; } }
		public override bool ColumnClearingSupported { get { return false; } }

		BufferredDataSource<ITuple> _DataSource;
		TuplesChunkReader _ChunkReader;


		/// <summary>
		/// Set <paramref name="chunkBufferSize"/> to a smaller value (like 10) if big jumps in the scrolling position are very frequent
		/// and your data source allows retrieving a few values, but frequently. Default is <see cref="BufferredDataSource{T}.BUFFER_MAX_SIZE_DEFAULT"/>. 
		/// <para>Though the best way is to find it by manually testing different values</para>
		/// </summary>
		public BufferredTableData(ITableColumns columnsProvider, int tuplesCount, TuplesChunkReader tuplesChunkReader, int chunkBufferSize, bool columnSortingSupported)
		{
			_ChunkReader = tuplesChunkReader;
			_DataSource = new BufferredDataSource<ITuple>(tuplesCount, ReadTuplesChunk, chunkBufferSize, false /*items will be created directly*/);

			Init(columnsProvider, columnSortingSupported);
		}

		void ReadTuplesChunk(ITuple[] into, int firstItemIndex, int countToRead)
		{
			_ChunkReader(into, firstItemIndex, countToRead);
		}
		/// <summary>
		/// Because this is backed by a <see cref="LazyList{T}"/>, the value is either returned (if exists) or created and then returned
		/// </summary>
		public override ITuple GetTuple(int index) { return _DataSource[index]; }

		/// <summary><see cref="BufferredDataSource{T}.TryGetCachedValue(int, out T)"/></summary>
		public bool TryGetCachedTuple(int index, out ITuple tuple) { return _DataSource.TryGetCachedValue(index, out tuple); }

		/// <summary>See <see cref="BufferredDataSource{T}.GetValueUnchecked(int)"/></summary>
		public ITuple GetExistingTuple(int index) { return _DataSource.GetValueUnchecked(index); }

		protected override bool SortTuplesListIfSupported(IComparer comparerToUse)
		{
			// No sorting for now, but can be done for smaller data sets. Will probably be implemented in a future version
			return false;
		}

		protected override bool ReverseTuplesListIfSupported()
		{
			// Also no reversing for now
			return false; 
		}
	}
}