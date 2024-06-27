#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace OS.Devices
{
	public sealed class RepeatableCancellationToken
	{
		private readonly MultiCancellationTokenSource tokenSource;
		private CancellationToken? token;

		public RepeatableCancellationToken(MultiCancellationTokenSource tokenSource)
		{
			this.tokenSource = tokenSource;
			RollToken();
		}

		public void ThrowIfCancellationRequested()
		{
			if (token == null)
			{
				RollToken();
				return;
			}

			if (token.Value.IsCancellationRequested)
			{
				RollToken();
				throw new TaskCanceledException();
			}
		}

		private void RollToken()
		{
			token = tokenSource.GetNewToken();
		}
	}
}