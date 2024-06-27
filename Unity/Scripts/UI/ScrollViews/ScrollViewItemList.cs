#nullable enable
using System.Collections;
using System.Collections.Generic;

namespace UI.ScrollViews
{
	public sealed class ScrollViewItemList<T> : IList<T>
	{
		private readonly IScrollViewController controller;
		private readonly List<T> dataList = new();
		
		public  ScrollViewItemList(IScrollViewController controller)
		{
			this.controller = controller;
		}

		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator()
		{
			return dataList.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(T item)
		{
			dataList.Add(item);
			controller.Refresh(dataList.Count);
		}

		/// <inheritdoc />
		public void Clear()
		{
			dataList.Clear();
			controller.Refresh(dataList.Count);
		}

		/// <inheritdoc />
		public bool Contains(T item)
		{
			return dataList.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(T[] array, int arrayIndex)
		{
			dataList.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(T item)
		{
			if (!dataList.Remove(item))
				return false;
			
			controller.Refresh(dataList.Count);
			return true;
		}

		/// <inheritdoc />
		public int Count => dataList.Count;

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		public int IndexOf(T item)
		{
			return dataList.IndexOf(item);
		}

		/// <inheritdoc />
		public void Insert(int index, T item)
		{
			dataList.Insert(index, item);
			controller.Refresh(dataList.Count);
		}

		/// <inheritdoc />
		public void RemoveAt(int index)
		{
			dataList.RemoveAt(index);
			controller.Refresh(dataList.Count);
		}

		/// <inheritdoc />
		public T this[int index]
		{
			get => dataList[index];
			set => dataList[index] = value;
		}

		public void SetItems(IEnumerable<T> items)
		{
			dataList.Clear();
			dataList.AddRange(items);
			controller.Refresh(dataList.Count);
		}
	}
}