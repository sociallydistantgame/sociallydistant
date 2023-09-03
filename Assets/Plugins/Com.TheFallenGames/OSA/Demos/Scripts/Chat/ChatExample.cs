using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

namespace Com.TheFallenGames.OSA.Demos.Chat
{
	/// <summary>This class demonstrates a basic chat implementation. A message can contain a text, image, or both</summary>
	public class ChatExample : OSA<MyParams, ChatMessageViewsHolder>
	{
		public SimpleDataHelper<ChatMessageModel> Data { get; private set; }


		#region OSA implementation
		protected override void Awake()
		{
			base.Awake();

			Data = new SimpleDataHelper<ChatMessageModel>(this);
		}

		/// <inheritdoc/>
		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			for (int i = 0; i < VisibleItemsCount; i++)
			{
				var visibleVH = GetItemViewsHolder(i);
				if (visibleVH.IsPopupAnimationActive)
					visibleVH.UpdatePopupAnimation(Time);
			}
		}

		/// <inheritdoc/>
		protected override ChatMessageViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new ChatMessageViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		/// <inheritdoc/>
		protected override void OnItemHeightChangedPreTwinPass(ChatMessageViewsHolder vh)
		{
			base.OnItemHeightChangedPreTwinPass(vh);

			Data[vh.ItemIndex].HasPendingVisualSizeChange = false;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(ChatMessageViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			ChatMessageModel model = Data[newOrRecycled.ItemIndex];

			newOrRecycled.UpdateFromModel(model, _Params);

			if (model.HasPendingVisualSizeChange)
			{
				// Height will be available before the next 'twin' pass, inside OnItemHeightChangedPreTwinPass() callback (see above)
				newOrRecycled.MarkForRebuild(); // will enable the content size fitter
												//newOrRecycled.contentSizeFitter.enabled = true;
				ScheduleComputeVisibilityTwinPass(true);
			}
			if (!newOrRecycled.IsPopupAnimationActive && newOrRecycled.itemIndexInView == GetItemsCount() - 1) // only animating the last one
				newOrRecycled.ActivatePopulAnimation(Time);
		}

		/// <inheritdoc/>
		protected override void OnBeforeRecycleOrDisableViewsHolder(ChatMessageViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			inRecycleBinOrVisible.DeactivatePopupAnimation();

			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
		}

		/// <inheritdoc/>
		protected override void RebuildLayoutDueToScrollViewSizeChange()
		{
			// Invalidate the last sizes so that they'll be re-calculated
			SetAllModelsHavePendingSizeChange();

			base.RebuildLayoutDueToScrollViewSizeChange();
		}

		/// <summary>
		/// When the user resets the count or refreshes, the OSA's cached sizes are cleared so we can recalculate them. 
		/// This is provided here for new users that just want to call Refresh() and have everything updated instead of telling OSA exactly what has updated.
		/// But, in most cases you shouldn't need to ResetItems() or Refresh() because of performace reasons:
		/// - If you add/remove items, InsertItems()/RemoveItems() is preferred if you know exactly which items will be added/removed;
		/// - When just one item has changed externally and you need to force-update its size, you'd call ForceRebuildViewsHolderAndUpdateSize() on it;
		/// - When the layout is rebuilt (when you change the size of the viewport or call ScheduleForceRebuildLayout()), that's already handled
		/// So the only case when you'll need to call Refresh() (and override ChangeItemsCount()) is if your models can be changed externally and you'll only know that they've changed, but won't know which ones exactly.
		/// </summary>
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			if (changeMode == ItemCountChangeMode.RESET)
				SetAllModelsHavePendingSizeChange();

			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}
		#endregion

		void SetAllModelsHavePendingSizeChange()
		{
			foreach (var model in Data)
				model.HasPendingVisualSizeChange = true;
		}
	}


	/// <summary><see cref="HasPendingVisualSizeChange"/> is set to true each time a property that can affect the height changes</summary>
	public class ChatMessageModel
	{
		public static readonly DateTime EPOCH_START_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

		public int timestampSec;

		public DateTime TimestampAsDateTime
		{
			get
			{
				// Unix timestamp is seconds past epoch
				System.DateTime dtDateTime = EPOCH_START_TIME.AddSeconds(timestampSec).ToLocalTime();
				return dtDateTime;
			}
		}
		public string Text
		{
			get { return _Text; }
			set
			{
				if (_Text == value)
					return;

				_Text = value;
				HasPendingVisualSizeChange = true;
			}
		}
		public int ImageIndex
		{
			get { return _ImageIndex; }
			set
			{
				if (_ImageIndex == value)
					return;

				_ImageIndex = value;
				HasPendingVisualSizeChange = true;
			}
		}
		public bool IsMine { get; set; }

		/// <summary>This will be true when the item size may have changed and the ContentSizeFitter component needs to be updated</summary>
		public bool HasPendingVisualSizeChange { get; set; }

		string _Text;
		int _ImageIndex;
	}


	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		public Sprite[] availableChatImages; // used to randomly generate models;
	}


	/// <summary>The ContentSizeFitter should be attached to the item itself</summary>
	public class ChatMessageViewsHolder : BaseItemViewsHolder
	{
		public Text timeText, text;
		public Image leftIcon, rightIcon;
		public Image image;
		public Image messageContentPanelImage;

		UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; set; }
		public float PopupAnimationStartTime { get; private set; }
		public bool IsPopupAnimationActive
		{
			get { return _IsAnimating; }
		}

		const float POPUP_ANIMATION_TIME = .2f;

		bool _IsAnimating;
		VerticalLayoutGroup _RootLayoutGroup, _MessageContentLayoutGroup;
		int paddingAtIconSide, paddingAtOtherSide;
		Color colorAtInit;


		public override void CollectViews()
		{
			base.CollectViews();

			_RootLayoutGroup = root.GetComponent<VerticalLayoutGroup>();
			paddingAtIconSide = _RootLayoutGroup.padding.right;
			paddingAtOtherSide = _RootLayoutGroup.padding.left;

			ContentSizeFitter = root.GetComponent<UnityEngine.UI.ContentSizeFitter>();
			ContentSizeFitter.enabled = false; // the content size fitter should not be enabled during normal lifecycle, only in the "Twin" pass frame
			root.GetComponentAtPath("MessageContentPanel", out _MessageContentLayoutGroup);
			messageContentPanelImage = _MessageContentLayoutGroup.GetComponent<Image>();
			messageContentPanelImage.transform.GetComponentAtPath("Image", out image);
			messageContentPanelImage.transform.GetComponentAtPath("TimeText", out timeText);
			messageContentPanelImage.transform.GetComponentAtPath("Text", out text);
			root.GetComponentAtPath("LeftIconImage", out leftIcon);
			root.GetComponentAtPath("RightIconImage", out rightIcon);
			colorAtInit = messageContentPanelImage.color;
		}

		public override void MarkForRebuild()
		{
			base.MarkForRebuild();
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = true;
		}

		public override void UnmarkForRebuild()
		{
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = false;
			base.UnmarkForRebuild();
		}

		/// <summary>Utility getting rid of the need of manually writing assignments</summary>
		public void UpdateFromModel(ChatMessageModel model, MyParams parameters)
		{
			timeText.text = model.TimestampAsDateTime.ToString("HH:mm");

			string messageText = "[#" + ItemIndex + "] " + model.Text;
			if (text.text != messageText)
				text.text = messageText;

			leftIcon.gameObject.SetActive(!model.IsMine);
			rightIcon.gameObject.SetActive(model.IsMine);
			if (model.ImageIndex < 0)
				image.gameObject.SetActive(false);
			else
			{
				image.gameObject.SetActive(true);
				image.sprite = parameters.availableChatImages[model.ImageIndex];
			}

			if (model.IsMine)
			{
				messageContentPanelImage.rectTransform.pivot = new Vector2(1.4f, .5f);
				messageContentPanelImage.color = new Color(.75f, 1f, 1f, colorAtInit.a);
				_RootLayoutGroup.childAlignment = _MessageContentLayoutGroup.childAlignment = text.alignment = TextAnchor.MiddleRight;
				_RootLayoutGroup.padding.right = paddingAtIconSide;
				_RootLayoutGroup.padding.left = paddingAtOtherSide;
			}
			else
			{
				messageContentPanelImage.rectTransform.pivot = new Vector2(-.4f, .5f);
				messageContentPanelImage.color = colorAtInit;
				_RootLayoutGroup.childAlignment = _MessageContentLayoutGroup.childAlignment = text.alignment = TextAnchor.MiddleLeft;
				_RootLayoutGroup.padding.right = paddingAtOtherSide;
				_RootLayoutGroup.padding.left = paddingAtIconSide;
			}
		}

		public void DeactivatePopupAnimation()
		{
			messageContentPanelImage.transform.localScale = Vector3.one;
			_IsAnimating = false;
		}

		public void ActivatePopulAnimation(float unityTime)
		{
			var s = messageContentPanelImage.transform.localScale;
			s.x = 0;
			messageContentPanelImage.transform.localScale = s;
			PopupAnimationStartTime = unityTime;
			_IsAnimating = true;
		}

		internal void UpdatePopupAnimation(float unityTime)
		{
			float elapsed = unityTime - PopupAnimationStartTime;
			float t01;
			if (elapsed > POPUP_ANIMATION_TIME)
				t01 = 1f;
			else
				// Normal in, sin slow out
				t01 = Mathf.Sin((elapsed / POPUP_ANIMATION_TIME) * Mathf.PI / 2);

			var s = messageContentPanelImage.transform.localScale;
			s.x = t01;
			messageContentPanelImage.transform.localScale = s;

			if (t01 == 1f)
				DeactivatePopupAnimation();

			//Debug.Log("Updating: " + itemIndexInView + ", t01=" + t01 + ", elapsed=" + elapsed);
		}
	}
}