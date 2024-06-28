#nullable enable

using frame8.ScrollRectItemsAdapter.Classic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.ScrollViews
{
	public abstract class ScrollViewController<TViewModel> : 
		ClassicSRIA<TViewModel>,
		IScrollViewController
		where TViewModel : ScrollViewModel
	{
		
		private RectTransform content = null!;

		
		private GameObject itemPrefab = null!;

		
		private Scrollbar scrollBar = null!;
		
		
		private bool isHorizontal = false;

		private bool isInitialized;
		
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ScrollViewController<TViewModel>));

			SetupScrollRect();
			
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			InitIfNeeded();
		}

		protected void InitIfNeeded()
		{
			if (isInitialized)
				return;

			base.Start();
			isInitialized = true;
		}

		/// <inheritdoc />
		protected override TViewModel CreateViewsHolder(int itemIndex)
		{
			TViewModel vh = CreateModel(itemIndex);

			vh.Init(this.itemPrefab, itemIndex);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(TViewModel vh)
		{
			this.UpdateModel(vh);
		}

		protected abstract TViewModel CreateModel(int index);
		protected abstract void UpdateModel(TViewModel model);

		/// <inheritdoc />
		public void Refresh(int itemCount)
		{
			InitIfNeeded();
			ResetItems(itemCount);
		}

		private void SetupScrollRect()
		{
			if (!this.TryGetComponent(out ScrollRect scrollRect))
				scrollRect = this.gameObject.AddComponent<ScrollRect>();

			scrollRect.content = this.content;
			scrollRect.viewport = this.viewport;
			scrollRect.horizontal = this.isHorizontal;

			if (scrollRect.horizontal)
				scrollRect.horizontalScrollbar = scrollBar;
			else scrollRect.verticalScrollbar = scrollBar;
		}
	}
}