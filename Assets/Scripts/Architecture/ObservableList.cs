#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

namespace Architecture
{
	public class ObservableList<T> : ICollection<T>
	{
		private readonly List<T> underlyingList = new List<T>();

		public event Action<T>? ItemAdded;
		public event Action<T>? ItemRemoved;

		public T this[int index]
		{
			get => this.underlyingList[index];
		}
		
		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator()
		{
			return this.underlyingList.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(T item)
		{
			this.underlyingList.Add(item);
			ItemAdded?.Invoke(item);
		}

		/// <inheritdoc />
		public void Clear()
		{
			while (this.underlyingList.Count > 0)
			{
				T item = underlyingList[0];
				Remove(item);
			}
		}

		/// <inheritdoc />
		public bool Contains(T item)
		{
			return underlyingList.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(T[] array, int arrayIndex)
		{
			underlyingList.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(T item)
		{
			bool result = underlyingList.Remove(item);
			if (result)
				ItemRemoved?.Invoke(item);
			return result;
		}

		/// <inheritdoc />
		public int Count => underlyingList.Count;

		/// <inheritdoc />
		public bool IsReadOnly => false;
	}
}