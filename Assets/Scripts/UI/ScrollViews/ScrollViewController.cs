#nullable enable

using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.ScrollViews
{
	public abstract class ScrollViewController<TViewModel> : 
		MonoBehaviour,
		IScrollViewController
		where TViewModel : ScrollViewModel
	{
		protected virtual void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ScrollViewController<TViewModel>));
		}

		public void Refresh(int itemCount)
		{
			
		}
		
		protected abstract TViewModel CreateModel(int itemIndex);
		protected abstract void UpdateModel(TViewModel model);
	}
}