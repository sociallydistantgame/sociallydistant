#nullable enable
namespace Core.Systems
{
	public sealed class Counter
	{
		private int value = 0;

		public int Value => value;
		
		public Counter(int value = 0)
		{
			this.value = value;
		}

		public void CountUp()
		{
			value++;
		}
		
		public void CountDown()
		{
			value--;
		}
		
	}
}