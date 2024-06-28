#nullable enable
namespace SociallyDistant.Architecture
{
	[Serializable]
	public class SerializableKeyValuePair<TKey, TValue>
	{
		
		private TKey key = default!;

		
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