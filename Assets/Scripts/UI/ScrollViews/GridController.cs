#nullable enable
using UnityEngine;
using UnityEngine.UIElements;
using UnityExtensions;

namespace UI.ScrollViews
{
	public abstract class GridController<TViewModel> : 
		MonoBehaviour,
		IScrollViewController
		where TViewModel : GridCellModel
	{
		[SerializeField]
		private ScrollView unityScrollView = null!;

		protected virtual void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(GridController<TViewModel>));
		}

		public void Refresh(int itemCount)
		{
			
		}
		
		protected abstract TViewModel CreateModel(int itemIndex);
		protected abstract void UpdateModel(TViewModel model);
	}
}