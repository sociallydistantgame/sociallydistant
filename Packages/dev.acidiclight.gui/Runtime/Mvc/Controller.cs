#nullable enable

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using AcidicGui.Common;

namespace AcidicGui.Mvc
{
	public abstract class Controller<TView> : MonoBehaviour
		where TView : IView
	{
		private readonly List<TView> viewsList = new List<TView>();

		protected int ViewsListCount => viewsList.Count;
		
		/// <summary>
		///		Gets a reference to the currently-visible view. If no view is visible, this will be null.
		/// </summary>
		protected TView? CurrentView
		{
			get
			{
				if (viewsList.Count == 0)
					return default;

				return viewsList[^1];
			}
		}

		
		/// <summary>
		///		Navigates to the specified view. When navigation is finished, an optional callback will be invoked.
		/// </summary>
		/// <param name="view">The view to navigate to.</param>
		/// <param name="callback">Optional callback to invoke when navigation has completed.</param>
		/// <remarks>
		///		<para>
		///			Navigating to a new view will hide the currently active view, if any, and push it to a stack.
		///			This allows you to create a menu system where each view represents a related screen within the menu.
		///			To return to the previous view, you can call the <see cref="GoBack"/> method.
		///		</para>
		///		<para>
		///			If you'd instead like to replace the current view or replace the entire view stack entirely,
		///			then call the <see cref="ReplaceCurrentView"/> or <see cref="ReplaceViews"/> methods respectively instead.
		///		</para>
		/// </remarks>
		protected void NavigateTo(TView view, Action? callback = null)
		{
			// Hide the current view, if any.
			if (CurrentView != null)
				CurrentView.Hide();
			
			// Set the new view as the current view by adding it to the list
			viewsList.Add(view);
			
			// Let there be light!
			view.Show(callback);
		}

		protected void ReplaceCurrentView(TView view, Action? callback = null)
		{
			// If there is no current view, then fall back to NavigateTo.
			if (CurrentView == null)
			{
				NavigateTo(view, callback);
				return;
			}
			
			// Otherwise, hide it, remove it, then add the new view.
			TView oldView = CurrentView;
			viewsList.RemoveAt(viewsList.Count - 1);
			oldView.Hide();
			
			viewsList.Add(view);
			view.Show(callback);
		}

		protected void ReplaceViews(TView view, Action? callback = null)
		{
			// If there are no views, fall back to NavigateTo
			if (CurrentView == null)
			{
				NavigateTo(view, callback);
				return;
			}
			
			// Hide the current view
			CurrentView.Hide();
			
			// Clear the views list
			viewsList.Clear();
			
			// Annnnd navigate to the new view!
			NavigateTo(view, callback);
		}

		protected void GoBack(Action? callback)
		{
			// If there isn't a view active, call the callback immediately.
			if (CurrentView == null)
			{
				callback?.Invoke();
				return;
			}
			
			// Grab the view before we remove it from the list.
			TView view = this.CurrentView;
            this.viewsList.RemoveAt(this.viewsList.Count - 1);
            
            // If there's another CurrentView, then we show it at the same time as hiding the previous view.
            // In this case, the callback fires when the new view finishes being shown.
            if (CurrentView != null)
            {
	            view.Hide();
	            CurrentView.Show(callback);
            }

            // Otherwise, we just hide the old view, and the callback fires when it finishes being hidden.
            else
            {
	            view.Hide(callback);
            }
		}

		protected async Task NavigateToAsync(TView view)
		{
			await AsyncExtensions.CallAsync(NavigateTo, view);
		}
		
		protected async Task ReplaceCurrentViewAsync(TView view)
		{
			await AsyncExtensions.CallAsync(ReplaceCurrentView, view);
		}
        
		protected async Task ReplaceViewsAsync(TView view)
		{
			await AsyncExtensions.CallAsync(ReplaceViews, view);
		}

		protected Task GoBackAsync()
		{
			var completionSource = new TaskCompletionSource<bool>();

			GoBack(() => completionSource.SetResult(true));
			
			return completionSource.Task;
		}
	}
}