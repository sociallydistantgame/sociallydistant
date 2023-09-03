using System;
using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;

namespace Com.TheFallenGames.OSA.CustomAdapters.DateTimePicker
{
	/// <summary>Implementing multiple adapters to get a generic picker which returns a <see cref="DateTime"/> object</summary>
	public class DateTimePickerAdapter : OSA<MyParams, MyItemViewsHolder>
	{
		public int SelectedValue { get; private set; }
		public event Action<int> OnSelectedValueChanged;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();
		}

		/// <inheritdoc/>
		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			if (VisibleItemsCount == 0)
				return;

			int middleVHIndex = VisibleItemsCount / 2;
			var middleVH = GetItemViewsHolder(middleVHIndex);

			var prevValue = SelectedValue;
			SelectedValue = _Params.GetItemValueAtIndex(middleVH.ItemIndex);
			middleVH.background.CrossFadeColor(_Params.selectedColor, .1f, false, false);

			for (int i = 0; i < VisibleItemsCount; ++i)
			{
				if (i != middleVHIndex)
					GetItemViewsHolder(i).background.CrossFadeColor(_Params.nonSelectedColor, .1f, false, false);
			}

			if (prevValue != SelectedValue && OnSelectedValueChanged != null)
				OnSelectedValueChanged(SelectedValue);
		}

		/// <inheritdoc/>
		protected override MyItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyItemViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyItemViewsHolder newOrRecycled) { newOrRecycled.titleText.text = _Params.GetItemValueAtIndex(newOrRecycled.ItemIndex) + ""; }
		#endregion

		void ChangeItemsCountWithChecks(int newCount)
		{
			int min = 4;
			if (newCount < min)
				newCount = min;

			ResetItems(newCount);
		}
	}


	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		public int startItemNumber = 0;
		public int increment = 1;
		public Color selectedColor, nonSelectedColor;

		/// <summary>The value of each item is calculated dynamically using its <paramref name="index"/>, <see cref="startItemNumber"/> and the <see cref="increment"/></summary>
		/// <returns>The item's value (the displayed number)</returns>
		public int GetItemValueAtIndex(int index) { return startItemNumber + increment * index; }
	}


	public class MyItemViewsHolder : BaseItemViewsHolder
	{
		public Image background;
		public Text titleText;

		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			background = root.GetComponent<Image>();
			titleText = root.GetComponentInChildren<Text>();
		}
	}
}
