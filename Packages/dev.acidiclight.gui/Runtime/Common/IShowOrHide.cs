#nullable enable

using System;

namespace AcidicGui.Common
{
	/// <summary>
	///		Interface for an object that has a visibility property and can be made visible or hidden.
	/// </summary>
	public interface IShowOrHide
	{
		/// <summary>
		///		Gets a value indicating whether the object is currently visible.
		/// </summary>
		bool IsVisible { get; }

		/// <summary>
		///		Make this object visible.
		/// </summary>
		/// <param name="callback">Optional callback to invoke when the object is fully visible.</param>
		void Show(Action? callback = null);
		
		/// <summary>
		///		Make this object hidden.
		/// </summary>
		/// <param name="callback">Optional callback to invoke when this object is fully hidden.</param>
		void Hide(Action? callback = null);
	}
}