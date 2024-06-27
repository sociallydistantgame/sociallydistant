#nullable enable

using System.Threading.Tasks;
using System;

namespace AcidicGui.Common
{
	/// <summary>
	///		Provides awaitable versions of methods within this framework that would otherwise take callbacks.
	/// </summary>
	public static class AsyncExtensions
	{
		public static Task ShowAsync(this IShowOrHide showOrHide)
		{
			var completionSource = new TaskCompletionSource<bool>();

			showOrHide.Show(() => completionSource.SetResult(true));
            
			return completionSource.Task;
		}
		
		public static Task HideAsync(this IShowOrHide showOrHide)
		{
			var completionSource = new TaskCompletionSource<bool>();

			showOrHide.Hide(() => completionSource.SetResult(true));
            
			return completionSource.Task;
		}

		public static Task CallAsync<TParam>(Action<TParam, Action?> methodWithCallback, TParam parameter)
		{
			var completionSource = new TaskCompletionSource<bool>();

			methodWithCallback(parameter, () => completionSource.SetResult(true));
            
			return completionSource.Task;
		}
	}
}