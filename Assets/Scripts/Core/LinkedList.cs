#nullable enable

using System.Collections;
using System.Collections.Generic;

namespace Core
{
	public class LinkedList<T> : ICollection<T>
	{
		private readonly EqualityComparer<T> comparer = EqualityComparer<T>.Default;
		private int count;
		private Node<T>? firstNode;
		private Node<T>? lastNode;
		
		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator()
		{
			return new NodeEnumerator<T>(this.firstNode);
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(T item)
		{
			if (firstNode == null)
			{
				firstNode = new Node<T>();
				firstNode.Value = item;
			}

			if (lastNode == null)
				lastNode = firstNode;
			else
			{
				var newNode = new Node<T>
				{
					Previous = lastNode,
					Value = item
				};

				lastNode.Next = newNode;
				lastNode = newNode;
			}

			count++;
		}

		/// <inheritdoc />
		public void Clear()
		{
			count = 0;
			firstNode = null;
			lastNode = null;
		}

		/// <inheritdoc />
		public bool Contains(T item)
		{
			if (firstNode == null)
				return false;

			foreach (T value in this)
			{
				if (comparer.Equals(item, value))
					return true;
			}

			return false;
		}

		/// <inheritdoc />
		public void CopyTo(T[] array, int arrayIndex)
		{
			var i = 0;
			foreach (T value in this)
			{
				if (i == array.Length)
					return;

				array[arrayIndex + i] = value;
				i++;
			}
		}

		/// <inheritdoc />
		public bool Remove(T item)
		{
			Node<T>? node = lastNode;

			if (node == null)
				return false;

			while (node != null)
			{
				if (comparer.Equals(node.Value, item))
				{
					if (node.Previous != null)
						node.Previous.Next = node.Next;

					if (node.Next == null)
						lastNode = node.Previous;

					node.Previous = null;
					node.Next = null;

					count--;
					return true;
				}
				
				node = node.Previous;
			}

			return false;
		}

		/// <inheritdoc />
		public int Count => count;

		/// <inheritdoc />
		public bool IsReadOnly => false;

		private class Node<T>
		{
			public Node<T>? Previous { get; set; }
			public Node<T>? Next { get; set; }
			public T Value { get; set; }
		}

		private class NodeEnumerator<T> : IEnumerator<T>
		{
			private Node<T>? head;
			private Node<T>? current;
			
			public NodeEnumerator(Node<T>? head)
			{
				this.head = head;
				this.current = head;
			}

			/// <inheritdoc />
			public bool MoveNext()
			{
				current = current?.Next;
				return current != null;
			}

			/// <inheritdoc />
			public void Reset()
			{
				current = head;
			}

			/// <inheritdoc />
			public T Current => current != null ? current.Value : default;

			/// <inheritdoc />
			object IEnumerator.Current => Current;

			/// <inheritdoc />
			public void Dispose()
			{
				head = null;
				current = null;
			}
		}
	}
}