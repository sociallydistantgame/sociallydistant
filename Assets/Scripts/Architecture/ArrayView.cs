#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Architecture
{
	public class ArrayView<TElementType>
	{
		private readonly TElementType[] collection;
		private int index;

		public int CurrentIndex => index;
		
		public bool EndOfArray => index >= collection.Length;

		public TElementType Current
		{
			get
			{
				ThrowIfEndOfCollection();
				return collection[index];
			}
		}

		public TElementType? Previous
		{
			get
			{
				if (index - 1 < 0)
					return default;

				return collection[index - 1];
			}
		}

		public TElementType? Next
		{
			get
			{
				if (index + 1 >= collection.Length)
					return default;

				return collection[index + 1];
			}
		}
		
		public ArrayView(TElementType[] collection)
		{
			this.collection = collection;
		}

		public void Advance()
		{
			ThrowIfEndOfCollection();
			index++;
		}

		public void GoToPrevious()
		{
			ThrowIfBeginOfCollection();
			index--;
		}

		private void ThrowIfBeginOfCollection()
		{
			if (index == 0)
				throw new InvalidOperationException("Beginning of collection");
		}
		
		private void ThrowIfEndOfCollection()
		{
			if (EndOfArray)
				throw new InvalidOperationException("End of collection");
		}
	}
}