using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Util.Animations;

namespace Com.TheFallenGames.OSA.Demos.Main
{
	/// <summary>
	/// <para>The main example implementation demonstrating common (not all) functionalities: </para>
	/// <para>- using both a horizontal (also includes optional snapping) and a vertical ScrollView with a complex prefab, </para>
	/// <para>- changing the item count, adding/removing to/from head/end of the list, </para>
	/// <para>- expanding/collapsing an item, thus demonstrating the possibility of multiple sizes, </para>
	/// <para>- smooth scrolling to an item &amp; optionally doing an action after the animation is done, </para>
	/// <para>- comparing the performance to the default implementation of a ScrollView,</para>
	/// <para>- the use of <see cref="frame8.Logic.Misc.Visual.UI.MonoBehaviours.ScrollbarFixer8"/></para>
	/// <para>At the core, everything's the same as in other example implementations, so if something's not clear, check them (SimpleExample is a good start)</para>
	/// </summary>
	public class MainExample : OSA<MyParams, ClientItemViewsHolder>
	{
		public LazyDataHelper<ClientModel> LazyData { get; private set; }

		ExpandCollapseAnimationState _ExpandCollapseAnimation;
		const float EXPAND_COLLAPSE_ANIM_DURATION = .2f;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			_Params.InitTextures();
			LazyData = new LazyDataHelper<ClientModel>(this, CreateNewModel);

			// Needed so that CancelUserAnimations() won't be called when sizes change (which happens during our animation itself)
			var cancel = _Params.Animation.Cancel;
			cancel.UserAnimations.OnSizeChanges = false;

			base.Start();
		}

		/// <inheritdoc/>
		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			if (_ExpandCollapseAnimation != null)
				AdvanceExpandCollapseAnimation();
		}

		/// <inheritdoc/>
		protected override void OnDestroy() { _Params.ReleaseTextures(); }

		/// <inheritdoc/>
		protected override ClientItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new ClientItemViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			if (_Params.itemsAreExpandable)
				instance.expandCollapseButton.onClick.AddListener(() => OnExpandCollapseButtonClicked(instance));

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(ClientItemViewsHolder newOrRecycled)
		{
			var model = LazyData.GetOrCreate(newOrRecycled.ItemIndex);
			newOrRecycled.UpdateViews(_Params, model);
		}

		/// <summary>
		/// <para>This is overidden only so that the items' title will be updated to reflect its new index in case of Insert/Remove, because the index is not stored in the model</para>
		/// <para>If you don't store/care about the index of each item, you can omit this</para>
		/// <para>For more info, see <see cref="OSA{TParams, TItemViewsHolder}.OnItemIndexChangedDueInsertOrRemove(TItemViewsHolder, int, bool, int)"/> </para>
		/// </summary>
		protected override void OnItemIndexChangedDueInsertOrRemove(ClientItemViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			shiftedViewsHolder.UpdateTitleByItemIndex(LazyData.GetOrCreate(shiftedViewsHolder.ItemIndex));
		}

		/// <inheritdoc/>
		protected override void CancelUserAnimations()
		{
			// Correctly handling OSA's request to stop user's (our) animations
			_ExpandCollapseAnimation = null;

			base.CancelUserAnimations();
		}
		#endregion

		void OnExpandCollapseButtonClicked(ClientItemViewsHolder vh)
		{
			// Force finish previous animation
			if (_ExpandCollapseAnimation != null)
			{
				int oldItemIndex = _ExpandCollapseAnimation.itemIndex;
				var oldModel = LazyData.GetOrCreate(oldItemIndex);
				_ExpandCollapseAnimation.ForceFinish();
				oldModel.ExpandedAmount = _ExpandCollapseAnimation.CurrentExpandedAmount;
				ResizeViewsHolderIfVisible(oldItemIndex, oldModel);
				_ExpandCollapseAnimation = null;
			}

			var model = LazyData.GetOrCreate(vh.ItemIndex);
			var anim = new ExpandCollapseAnimationState(_Params.UseUnscaledTime);
			anim.initialExpandedAmount = model.ExpandedAmount;
			anim.duration = EXPAND_COLLAPSE_ANIM_DURATION;
			if (model.ExpandedAmount == 1f) // fully expanded
				anim.targetExpandedAmount = 0f;
			else
				anim.targetExpandedAmount = 1f;

			anim.itemIndex = vh.ItemIndex;

			_ExpandCollapseAnimation = anim;
		}

		float GetModelExpandedSize()
		{
			return _Params.expandFactor * _Params.ItemPrefabSize;
		}

		float GetModelCurrentSize(ClientModel model)
		{
			float expandedSize = GetModelExpandedSize();

			return Mathf.Lerp(_Params.DefaultItemSize, expandedSize, model.ExpandedAmount);
		}

		void ResizeViewsHolderIfVisible(int itemIndex, ClientModel model)
		{
			float newSize = GetModelCurrentSize(model);

			// Set to true if positions aren't corrected; this happens if you don't position the pivot exactly at the stationary edge
			bool correctPositions = false;

			RequestChangeItemSizeAndUpdateLayout(itemIndex, newSize, false, true, correctPositions);

			var vh = GetItemViewsHolderIfVisible(itemIndex);
			if (vh != null)
			{
				// Fixing Unity bug: https://issuetracker.unity3d.com/issues/rectmask2d-doesnt-update-when-the-parent-size-is-changed
				// Changing the transform's scale and restoring it back. This trigggers the update of the RectMask2D. 
				// Tried RectMask2D.PerformClipping(), tried setting m_ForceClipping and other params through reflection with no success.
				// This workaround remains the only one that works.
				// This is not needed in case galleryEffect is bigger than 0, since that already changes the items' scale periodically, but we included it to cover all cases
				var localScale = vh.rectMask2DRectTransform.localScale;
				vh.rectMask2DRectTransform.localScale = localScale * .99f;
				vh.rectMask2DRectTransform.localScale = localScale;
			}
		}

		void AdvanceExpandCollapseAnimation()
		{
			int itemIndex = _ExpandCollapseAnimation.itemIndex;
			var model = LazyData.GetOrCreate(itemIndex);
			model.ExpandedAmount = _ExpandCollapseAnimation.CurrentExpandedAmount;
			ResizeViewsHolderIfVisible(itemIndex, model);

			if (_ExpandCollapseAnimation != null && _ExpandCollapseAnimation.IsDone)
				_ExpandCollapseAnimation = null;
		}

		ClientModel CreateNewModel(int index)
		{
			var model = new ClientModel()
			{
				avatarImageId = Rand(_Params.sampleAvatars.Count),
				//clientName = _Params.sampleFirstNames[Rand(_Params.sampleFirstNames.Length)] + _Params.sampleLastNames[Rand(_Params.sampleLastNames.Length)],
				clientName = _Params.sampleFirstNames[Rand(_Params.sampleFirstNames.Length)],
				//clientName = "Client #" + index,
				location = _Params.sampleLocations[Rand(_Params.sampleLocations.Length)],
				availability01 = RandF(),
				contractChance01 = RandF(),
				longTermClient01 = RandF(),
				isOnline = Rand(2) == 0
			};

			int friendsCount = Rand(10);
			model.friendsAvatarIds = new int[friendsCount];
			for (int i = 0; i < friendsCount; i++)
				model.friendsAvatarIds[i] = Rand(_Params.sampleAvatarsDownsized.Count);

			return model;
		}

		// Utility randomness methods
		int Rand(int maxExcl) { return UnityEngine.Random.Range(0, maxExcl); }
		float RandF(float maxExcl = 1f) { return UnityEngine.Random.Range(0, maxExcl); }
    }


	public class ClientModel
	{
		/// <summary>In a real-world scenario, will be used to retrieve the actual image from an image cacher. Here, it's used as the avatar's index in params' sampleAvatars list</summary>
		public int avatarImageId;
		public int[] friendsAvatarIds; // actually avatar indices
		public string clientName;
		public string location;
		public float availability01, contractChance01, longTermClient01;
		public bool isOnline;

		public float AverageScore01 { get { return (availability01 + contractChance01 + longTermClient01) / 3; } }

		// View size related
		public float ExpandedAmount { get; set; }
	}


	public class ClientItemViewsHolder : BaseItemViewsHolder
	{
		public Image avatarImage, averageScoreFillImage;
		public Text nameText, locationText, averageScoreText, friendsText;
		public RectTransform availability01Slider, contractChance01Slider, longTermClient01Slider;
		public Text statusText;
		public Transform[] friendsPanels = new Transform[MAX_DISPLAYED_FRIENDS];
		public CanvasGroup[] friendsPanelsCanvasGroups = new CanvasGroup[MAX_DISPLAYED_FRIENDS];
		public Image[] friendsAvatarImages = new Image[MAX_DISPLAYED_FRIENDS];
		public Button expandCollapseButton;
		public RectTransform rectMask2DRectTransform;

		const int MAX_DISPLAYED_FRIENDS = 5;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			// The RectMask2D is on the root
			rectMask2DRectTransform = root;

			var mainPanel = root.GetChild(0);
			mainPanel.GetComponentAtPath("AvatarPanel", out avatarImage);
			mainPanel.GetComponentAtPath("AvatarPanel/StatusText", out statusText);
			mainPanel.GetComponentAtPath("NameAndLocationPanel/NameText", out nameText);
			mainPanel.GetComponentAtPath("NameAndLocationPanel/LocationText", out locationText);

			var friendsPanel = mainPanel.GetComponentAtPath<RectTransform>("FriendsPanel");
			for (int i = 0; i < MAX_DISPLAYED_FRIENDS; i++)
			{
				var ch = friendsPanels[i] = friendsPanel.GetChild(i);
				friendsPanelsCanvasGroups[i] = ch.GetComponent<CanvasGroup>();
				friendsAvatarImages[i] = ch.GetComponent<Image>();
			}
			friendsPanel.GetComponentAtPath("FriendsText", out friendsText);

			var ratingPanel = root.GetComponentAtPath<RectTransform>("RatingPanel/Panel");
			ratingPanel.GetComponentAtPath("Foreground", out averageScoreFillImage);
			ratingPanel.GetComponentAtPath("Text", out averageScoreText);

			var ratingBreakdownPanel = root.GetComponentAtPath<RectTransform>("RatingBreakdownPanel");
			ratingBreakdownPanel.GetComponentAtPath("AvailabilityPanel/Slider", out availability01Slider);
			ratingBreakdownPanel.GetComponentAtPath("ContractChancePanel/Slider", out contractChance01Slider);
			ratingBreakdownPanel.GetComponentAtPath("LongTermClientPanel/Slider", out longTermClient01Slider);

			expandCollapseButton = root.GetComponent<Button>();
		}

		public void UpdateViews(MyParams p, ClientModel dataModel)
		{
			avatarImage.sprite = p.sampleAvatars[dataModel.avatarImageId];
			UpdateTitleByItemIndex(dataModel);
			locationText.text = "  " + dataModel.location;
			UpdateScores(dataModel);
			friendsText.text = dataModel.friendsAvatarIds.Length + (dataModel.friendsAvatarIds.Length == 1 ? " friend" : " friends");
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

			UpdateFriendsAvatars(dataModel, p);
		}

		public void UpdateTitleByItemIndex(ClientModel dataModel)
		{
			nameText.text = dataModel.clientName + "(#" + ItemIndex + ")";
		}

		void UpdateScores(ClientModel dataModel)
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

		void UpdateFriendsAvatars(ClientModel dataModel, MyParams p)
		{
			// Set avatars for friends + set their alpha to 1;
			int i = 0;
			int friendsCount = dataModel.friendsAvatarIds.Length;
			int limit = Mathf.Min(MAX_DISPLAYED_FRIENDS, friendsCount);
			for (; i < limit; i++)
			{
				friendsAvatarImages[i].sprite = p.sampleAvatarsDownsized[dataModel.friendsAvatarIds[i]];
				//friendsPanels[i].gameObject.SetActive(true);
				friendsPanels[i].localScale = Vector3.one;
			}

			// Hide the rest
			for (; i < MAX_DISPLAYED_FRIENDS; ++i)
			{
				//friendsPanels[i].gameObject.SetActive(false);
				friendsPanels[i].localScale = Vector3.zero;
			}

			// .. but fade the last 2, if the friends count is big enough
			if (friendsCount > MAX_DISPLAYED_FRIENDS - 1)
				friendsPanelsCanvasGroups[MAX_DISPLAYED_FRIENDS - 1].alpha = .1f;
			if (friendsCount > MAX_DISPLAYED_FRIENDS - 2)
				friendsPanelsCanvasGroups[MAX_DISPLAYED_FRIENDS - 2].alpha = .4f;
		}
	}


	[Serializable]
	public class MyParams : BaseParamsWithPrefab
	{
		public List<Sprite> sampleAvatars;
		public string[] sampleFirstNames;//, sampleLastNames;
		public string[] sampleLocations;
		public bool itemsAreExpandable;
		public float expandFactor = 2f;

		[NonSerialized]
		public List<Sprite> sampleAvatarsDownsized;

		[NonSerialized]
		public bool freezeItemEndEdgeWhenResizing;


		// Creating sprites with down-sized textures for friends' avatars and new sprites for bigger avatars with 
		// mipmaps turned off (so they'll always look sharp)
		internal void InitTextures()
		{
			sampleAvatarsDownsized = new List<Sprite>(sampleAvatars.Count);
			for (int i = 0; i < sampleAvatars.Count; i++)
			{
				var avatar = sampleAvatars[i];
				var mipPixels = avatar.texture.GetPixels32(Mathf.Min(2, avatar.texture.mipmapCount));
				int len = (int)Math.Sqrt(mipPixels.Length);
				var t = new Texture2D(len, len, TextureFormat.RGBA32, false);
				t.SetPixels32(mipPixels);
				t.Apply();
				var sprite = Sprite.Create(t, new Rect(0, 0, len, len), Vector2.one * .5f);
				sampleAvatarsDownsized.Add(sprite);

				var noMipMapTex = new Texture2D(avatar.texture.width, avatar.texture.height, TextureFormat.RGBA32, false);
				noMipMapTex.SetPixels32(avatar.texture.GetPixels32());
				noMipMapTex.Apply();
				sampleAvatars[i] = Sprite.Create(noMipMapTex, avatar.textureRect, Vector2.one * .5f);
			}
		}

		internal void ReleaseTextures()
		{
			for (int i = 0; i < sampleAvatars.Count; i++)
			{
				var av = sampleAvatars[i];
				if (av)
					GameObject.Destroy(av);
			}
			for (int i = 0; i < sampleAvatarsDownsized.Count; i++)
			{
				var av = sampleAvatarsDownsized[i];
				if (av)
					GameObject.Destroy(av);
			}
		}
	}
}
