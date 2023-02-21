#nullable enable
using System;
using UnityEngine;

namespace Architecture
{
	[Serializable]
	public class SerializableKeyValuePair<TKey, TValue>
	{
		[SerializeField]
		private TKey key = default!;

		[SerializeField]
		private TValue value = default!;

		public TKey Key
		{
			get => key;
			set => key = value;
		}

		public TValue Value
		{
			get => this.value;
			set => this.value = value; // my brain, it hurts. <3
		}
	}
}