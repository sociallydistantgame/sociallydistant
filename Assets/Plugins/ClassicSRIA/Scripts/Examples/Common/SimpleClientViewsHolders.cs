using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Classic.Util;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples.Common
{
	public class BaseClientViewsHolder<TClientModel> : CAbstractViewsHolder where TClientModel : SimpleClientModel
	{
		public LayoutElement layoutElement;
		public Image averageScoreFillImage;
		public Text nameText, locationText, averageScoreText;
		public RectTransform availability01Slider, contractChance01Slider, longTermClient01Slider;
		public Text statusText;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			layoutElement = root.GetComponent<LayoutElement>();

			var mainPanel = root.GetChild(0);
			statusText = mainPanel.Find("AvatarPanel/StatusText").GetComponent<Text>();
			nameText = mainPanel.Find("NameAndLocationPanel/NameText").GetComponent<Text>();
			locationText = mainPanel.Find("NameAndLocationPanel/LocationText").GetComponent<Text>();

			var ratingPanel = root.Find("RatingPanel/Panel").GetComponent<RectTransform>();
			averageScoreFillImage = ratingPanel.Find("Foreground").GetComponent<Image>();
			averageScoreText = ratingPanel.Find("Text").GetComponent<Text>();

			var ratingBreakdownPanel = root.Find("RatingBreakdownPanel").GetComponent<RectTransform>();
			availability01Slider = ratingBreakdownPanel.Find("AvailabilityPanel/Slider").GetComponent<RectTransform>();
			contractChance01Slider = ratingBreakdownPanel.Find("ContractChancePanel/Slider").GetComponent<RectTransform>();
			longTermClient01Slider = ratingBreakdownPanel.Find("LongTermClientPanel/Slider").GetComponent<RectTransform>();
		}

		public virtual void UpdateViews(TClientModel dataModel)
		{
			nameText.text = dataModel.clientName + "(#" + ItemIndex + ")";
			locationText.text = "  " + dataModel.location;
			UpdateScores(dataModel);
			if (dataModel.isOnline)
			{
				statusText.text = "Online";
				statusText.color = Color.green;
			}
			else
			{
				statusText.text = "Offline";
				statusText.color = Color.white * .8f;
			}
		}

		void UpdateScores(SimpleClientModel dataModel)
		{
			var scale = availability01Slider.localScale;
			scale.x = dataModel.availability01;
			availability01Slider.localScale = scale;

			scale = contractChance01Slider.localScale;
			scale.x = dataModel.contractChance01;
			contractChance01Slider.localScale = scale;

			scale = longTermClient01Slider.localScale;
			scale.x = dataModel.longTermClient01;
			longTermClient01Slider.localScale = scale;

			float avgScore = dataModel.AverageScore01;
			averageScoreFillImage.fillAmount = avgScore;
			averageScoreText.text = (int)(avgScore * 100) + "%";
		}
	}

	public class SimpleClientViewsHolder : BaseClientViewsHolder<SimpleClientModel>
	{

	}


	public class SimpleExpandableClientViewsHolder : BaseClientViewsHolder<ExpandableSimpleClientModel>
	{
		public CExpandCollapseOnClick expandCollapseComponent;


		public override void CollectViews()
		{
			base.CollectViews();

			expandCollapseComponent = root.GetComponent<CExpandCollapseOnClick>();
		}

		public override void UpdateViews(ExpandableSimpleClientModel dataModel)
		{
			base.UpdateViews(dataModel);

			if (expandCollapseComponent)
			{
				expandCollapseComponent.expanded = dataModel.expanded;
				expandCollapseComponent.nonExpandedSize = dataModel.nonExpandedSize;
			}
		}
	}
}
