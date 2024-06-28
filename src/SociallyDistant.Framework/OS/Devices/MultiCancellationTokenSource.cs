#nullable enable
using System.Collections.Concurrent;

namespace SociallyDistant.Core.OS.Devices
{
	public sealed class MultiCancellationTokenSource
	{
		private readonly ConcurrentBag<CancellationTokenSource> tokenSources = new ConcurrentBag<CancellationTokenSource>();

		public void CancelAll()
		{
			foreach (CancellationTokenSource tokenSource in tokenSources)
			{
				tokenSource.Cancel();
			}

			tokenSources.Clear();
		}
		
		public CancellationToken GetNewToken()
		{
			var source = new CancellationTokenSource();
			
			tokenSources.Add(source);

			return source.Token;
		}
	}
}