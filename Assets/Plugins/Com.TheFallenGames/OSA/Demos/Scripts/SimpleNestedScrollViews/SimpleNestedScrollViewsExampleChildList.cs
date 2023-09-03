using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

namespace Com.TheFallenGames.OSA.Demos.SimpleNestedScrollViews
{
	/// <summary>The OSA adapter handling the list of items on a page</summary>
	public class SimpleNestedScrollViewsExampleChildList : OSA<BaseParamsWithPrefab, ChildListItemVH>
	{
		public SimpleDataHelper<ChildListItemModel> Data { get; private set; }


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Awake()
		{
			Data = new SimpleDataHelper<ChildListItemModel>(this);

			base.Awake();
		}

		/// <inheritdoc/>
		protected override ChildListItemVH CreateViewsHolder(int itemIndex)
		{
			var instance = new ChildListItemVH();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(ChildListItemVH newOrRecycled)
		{
			var model = Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);
		}
		#endregion
	}


	public class ChildListItemModel
	{
		public string Text { get; set; }
	}


	public class ChildListItemVH : BaseItemViewsHolder
	{
		Text _TitleText;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			root.GetComponentAtPath("TitleText", out _TitleText);
		}

		public void UpdateViews(ChildListItemModel model)
		{
			_TitleText.text = model.Text;
		}
	}
}
