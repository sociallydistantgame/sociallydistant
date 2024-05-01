using System;

namespace GameplaySystems.Networld
{
	public class Edge<T1, T2>
	{
		public T1? Side1 { get; set; }
		public T2? Side2 { get; set; }

		public Guid Token { get; } = Guid.NewGuid();
	}
}