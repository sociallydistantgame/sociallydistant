using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Util.Animations;

namespace Com.TheFallenGames.OSA.Demos.SelectAndDelete
{
    /// <summary>
    /// Implementation demonstrating the usage of a <see cref="GridAdapter{TParams, TCellVH}"/> with support for selecting items on long click and deleting them with a nice collapse animation
    /// </summary>
    public class SelectAndDeleteExample : GridAdapter<MyGridParams, MyCellViewsHolder>, LongClickableItem.IItemLongClickListener
	{
		public event Action<bool> SelectionModeChanged;

		// Initialized outside
		public LazyDataHelper<BasicModel> LazyData { get; set; }
		
		bool WaitingForItemsToBeDeleted { get { return _VisibleItemsAnimatedPreDeletion.Count > 0; } }

		ExpandCollapseAnimationState _PreDeleteAnimation;
		bool _SelectionMode;
		List<BasicModel> _SelectedModels = new List<BasicModel>();
		List<MyCellViewsHolder> _VisibleItemsAnimatedPreDeletion = new List<MyCellViewsHolder>();


		#region GridAdapter implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			// Prevent initialization. It'll be done from the outside
			//base.Start();

			var cancel = _Params.Animation.Cancel;
			// Needed so that CancelUserAnimations() won't be called when sizes change (which happens during our animation itself)
			cancel.UserAnimations.OnCountChanges = false;
			// Needed so that CancelUserAnimations() won't be called on count changes - we're handling these manually by overriding ChangeItemsCount 
			cancel.UserAnimations.OnSizeChanges = false;

			_Params.deleteButton.onClick.AddListener(() => { if (!PlayPreDeleteAnimation()) DeleteSelectedItems(); }); // delete items directly, if selected item is visible to wait for the delete animation
			_Params.cancelButton.onClick.AddListener(() => SetSelectionModeWithChecks(false));
		}

		/// <inheritdoc/>
		protected override void Update()
		{
			base.Update();

			if (!IsInitialized)
				return;

			if (Input.GetKeyUp(KeyCode.Escape))
				SetSelectionModeWithChecks(false);

			if (_PreDeleteAnimation != null)
				AdvancePreDeleteAnimation();
		}

		/// <inheritdoc/>
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int cellsCount, int indexIfAppendingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			// Assure nothing is selected before changing the count
			// Update: not calling RefreshSelectionStateForVisibleCells(), since UpdateCellViewsHolder() will be called for all cells anyway
			if (_SelectionMode)
				SetSelectionMode(false);
			UpdateSelectionActionButtons();

			base.ChangeItemsCount(changeMode, cellsCount, indexIfAppendingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}

		/// <seealso cref="GridAdapter{TParams, TCellVH}.Refresh(bool, bool)"/>
		public override void Refresh(bool contentPanelEndEdgeStationary = false /*ignored*/, bool keepVelocity = false)
		{
			_CellsCount = LazyData.Count;
			base.Refresh(_Params.freezeContentEndEdgeOnCountChange, keepVelocity);
		}

		protected override void OnCellViewsHolderCreated(MyCellViewsHolder cellVH, CellGroupViewsHolder<MyCellViewsHolder> cellGroup)
		{
			base.OnCellViewsHolderCreated(cellVH, cellGroup);

			// Set listeners for the Toggle in each cell. Will call OnCellToggled() when the toggled state changes
			// Set this adapter as listener for the OnItemLongClicked event
			cellVH.toggle.onValueChanged.AddListener(_ => OnCellToggled(cellVH));
			cellVH.longClickableComponent.longClickListener = this;
		}

		/// <summary> Called when a cell becomes visible </summary>
		/// <param name="viewsHolder"> use viewsHolder.ItemIndexto find your corresponding model and feed data into its views</param>
		/// <see cref="GridAdapter{TParams, TCellVH}.UpdateCellViewsHolder(TCellVH)"/>
		protected override void UpdateCellViewsHolder(MyCellViewsHolder viewsHolder)
		{
			var model = LazyData.GetOrCreate(viewsHolder.ItemIndex);
			viewsHolder.UpdateViews(model);

			UpdateSelectionState(viewsHolder, model);
			viewsHolder.UpdateViewsScale(model);
		}

		/// <inheritdoc/>
		protected override void OnBeforeRecycleOrDisableCellViewsHolder(MyCellViewsHolder viewsHolder, int newItemIndex)
		{
			viewsHolder.views.localScale = Vector3.one;
		}

		/// <inheritdoc/>
		protected override void CancelUserAnimations()
		{
			// Correctly handling OSA's request to stop user's (our) animations
			_PreDeleteAnimation = null;

			base.CancelUserAnimations();
		}
		#endregion

		#region LongClickableItem.IItemLongClickListener implementation
		public void OnItemLongClicked(LongClickableItem longClickedItem)
		{
			// Enter selection mode
			SetSelectionMode(true);
			RefreshSelectionStateForVisibleCells();
			UpdateSelectionActionButtons();

			if (_Params.autoSelectFirstOnSelectionMode)
			{
				// Find the cell views holder that corresponds to the LongClickableItem parameter & mark it as toggled
				int numVisibleCells = GetNumVisibleCells();
				for (int i = 0; i < numVisibleCells; ++i)
				{
					var cellVH = base.GetCellViewsHolder(i);

					if (cellVH.longClickableComponent == longClickedItem)
					{
						var model = LazyData.GetOrCreate(cellVH.ItemIndex);
						SelectModel(model);
						UpdateSelectionState(cellVH, model);

						break;
					}
				}
			}
		}
		#endregion

		void UpdateSelectionState(MyCellViewsHolder viewsHolder, BasicModel model)
		{
			viewsHolder.longClickableComponent.gameObject.SetActive(!_SelectionMode); // can be long-clicked only if selection mode is off
			viewsHolder.toggle.gameObject.SetActive(_SelectionMode); // can be selected only if selection mode is on
			viewsHolder.toggle.isOn = model.isSelected;
		}

		void SetSelectionModeWithChecks(bool isSelectionMode)
		{
			if (_SelectionMode != isSelectionMode)
			{
				SetSelectionMode(isSelectionMode);
				RefreshSelectionStateForVisibleCells();
				UpdateSelectionActionButtons();
			}
		}

		/// <summary>Assumes the current state of SelectionMode is different than <paramref name="active"/></summary>
		void SetSelectionMode(bool active)
		{
			_SelectionMode = active;
			UnselectAllModels();
			_VisibleItemsAnimatedPreDeletion.Clear();

			if (SelectionModeChanged != null)
				SelectionModeChanged(active);
		}

		void UpdateSelectionActionButtons()
		{
			if (!_SelectionMode)
				_Params.deleteButton.interactable = false;

			_Params.cancelButton.interactable = _SelectionMode;
		}

		bool PlayPreDeleteAnimation()
		{
			// Force finish previous animation
			if (_PreDeleteAnimation != null)
				throw new InvalidOperationException("Previous delete animation not finished");

			var numVisibleCells = GetNumVisibleCells();
			for (int i = 0; i < numVisibleCells; ++i)
			{
				var cellVH = GetCellViewsHolder(i);
				if (cellVH == null)
					continue;

				var m = LazyData.GetOrCreate(cellVH.ItemIndex);
				if (!m.isSelected) // faster to use the isSelected flag than to search through _SelectedModels
					continue;

				_VisibleItemsAnimatedPreDeletion.Add(cellVH);
			}

			if (_VisibleItemsAnimatedPreDeletion.Count == 0)
				return false;

			_Params.deleteButton.interactable = false;

			var anim = new ExpandCollapseAnimationState(_Params.UseUnscaledTime);
			anim.initialExpandedAmount = 1f;
			anim.duration = .2f;
			anim.targetExpandedAmount = 0f;
			anim.itemIndex = -1; // not needed in our case

			_PreDeleteAnimation = anim;

			return true;
		}

		void RefreshSelectionStateForVisibleCells()
		{
			// Rather than calling Refresh, we retrieve the already-visible ones and update them manually (less lag)
			int visibleCellCount = GetNumVisibleCells();
			for (int i = 0; i < visibleCellCount; ++i)
			{
				var cellVH = GetCellViewsHolder(i);
				UpdateSelectionState(cellVH, LazyData.GetOrCreate(cellVH.ItemIndex));
			}
		}

		void OnCellToggled(MyCellViewsHolder cellVH)
		{
			// Update the model this cell is representing
			var model = LazyData.GetOrCreate(cellVH.ItemIndex);
			if (cellVH.toggle.isOn)
				SelectModel(model);
			else
				UnselectModel(model);
			cellVH.UpdateViewsScale(model);
		}

		void SelectModel(BasicModel model)
		{
			if (!model.isSelected)
			{
				_SelectedModels.Add(model);
				model.isSelected = true;
			}
			_Params.deleteButton.interactable = true;
		}

		void UnselectModel(BasicModel model)
		{
			if (model.isSelected)
			{
				_SelectedModels.Remove(model);
				model.isSelected = false;
			}

			// Activate the delete button if at least one item was selected
			_Params.deleteButton.interactable = _SelectedModels.Count > 0;
		}

		// Faster than removing the models one by one using UnselectModel
		void UnselectAllModels()
		{
			for (int i = 0; i < _SelectedModels.Count; ++i)
				_SelectedModels[i].isSelected = false;
			_SelectedModels.Clear();
			_Params.deleteButton.interactable = false;
		}

		/// <summary>Deletes the selected items immediately</summary>
		void DeleteSelectedItems()
		{
			if (_SelectedModels.Count > 0)
			{
				// Remove models from adapter & update views
				// TODO see how necessary is to use an id-based approach to remove the items even faster frm a map instead of iterating through the list
				for (int i = 0; i < _SelectedModels.Count; ++i)
					LazyData.List.Remove(_SelectedModels[i]);

				// Re-enable selection mode
				if (_Params.keepSelectionModeAfterDeletion)
					SetSelectionModeWithChecks(true);

				// "Remove from disk" or similar
				foreach (var item in _SelectedModels)
					HandleItemDeletion(item);

				UnselectAllModels();

				Refresh(_Params.freezeContentEndEdgeOnCountChange);
			}
		}

		void AdvancePreDeleteAnimation()
		{
			if (_PreDeleteAnimation == null)
				return;

			foreach (var collapsingVH in _VisibleItemsAnimatedPreDeletion)
			{
				int itemIndex = collapsingVH.ItemIndex;
				var model = LazyData.GetOrCreate(itemIndex);
				model.ExpandedSubAmount = _PreDeleteAnimation.CurrentExpandedAmount;
				collapsingVH.UpdateViewsScale(model);
			}

			if (_PreDeleteAnimation.IsDone)
			{
				_VisibleItemsAnimatedPreDeletion.Clear();
				DeleteSelectedItems();
				_PreDeleteAnimation = null;
			}
		}


		void HandleItemDeletion(BasicModel model)
        { Debug.Log("Deleted with id: " + model.id); }
	}


	[Serializable]
	public class MyGridParams : GridParams
	{
		/// <summary>Will be enabled when in selection mode and there are items selsted. Disabled otherwise</summary>
		public Button deleteButton;

		/// <summary>Will be enabled when in selection mode. Pressing it will exit selection mode. Useful for devices with no back/escape (iOS)</summary>
		public Button cancelButton;

		/// <summary>Select the first item when entering selection mode</summary>
		public bool autoSelectFirstOnSelectionMode = true;

		/// <summary>Wether to remain in selection mode after deletion or not</summary>
		public bool keepSelectionModeAfterDeletion = true;

		[NonSerialized]
		public bool freezeContentEndEdgeOnCountChange;
	}


	public class BasicModel
	{
		public const float SELECTED_SCALE_FACTOR = .8f;

		// Data state
		public int id;
		public Color color;

		// View state
		public bool isSelected;

		/// <summary>
		/// Ranging form 0 to 1.
		/// The term Sub-Amount is used because the final Amount is also controlled by <see cref="isSelected"/>.
		/// For example, when <see cref="isSelected"/> is true, the final amount is <see cref="ExpandedSubAmount"/> * <see cref="SELECTED_SCALE_FACTOR"/>; otherwise it's <see cref="ExpandedSubAmount"/>.
		/// </summary>
		public float ExpandedSubAmount { get; set; }

		public float ExpandedRealAmount { get { return isSelected ? ExpandedSubAmount * SELECTED_SCALE_FACTOR : ExpandedSubAmount; } }


		public BasicModel()
		{
			ExpandedSubAmount = 1f; // All cells are initially fully visible
		}
	}


	/// <summary>All views holders used with GridAdapter should inherit from <see cref="CellViewsHolder"/></summary>
	public class MyCellViewsHolder : CellViewsHolder
	{
		public Text title;
		public Toggle toggle;
		public LongClickableItem longClickableComponent;
		public Image background;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			toggle = views.Find("Toggle").GetComponent<Toggle>();
			title = views.Find("TitleText").GetComponent<Text>();
			longClickableComponent = views.Find("LongClickableArea").GetComponent<LongClickableItem>();
			background = views.GetComponent<Image>();
		}

		public void UpdateViews(BasicModel model)
		{
			title.text = "#" + ItemIndex + " [id:" + model.id + "]";
			background.color = model.color;
		}

		public void UpdateViewsScale(BasicModel model)
		{
			views.localScale = Vector3.one * model.ExpandedRealAmount;
		}
	}
}