using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;

namespace Com.TheFallenGames.OSA.Demos.Hierarchy
{
	/// <summary>Demonstrating a hierarchy view (aka Tree View) implemented with OSA</summary>
	public class HierarchyExample : OSA<MyParams, FileSystemEntryViewsHolder>, IHierarchyOSA
	{
		bool _BusyWithAnimation;


		#region OSA implementation
		/// <inheritdoc/>
		protected override FileSystemEntryViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new FileSystemEntryViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			instance.SetOnToggleFoldoutListener(OnDirectoryFoldOutClicked);

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(FileSystemEntryViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			IHierarchyNodeModel model = _Params.FlattenedVisibleHierarchy[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);
		}
		#endregion

		#region UI
		public bool TryGenerateRandomTree(int[] childCountsPerDepth = null)
		{
			if (_BusyWithAnimation)
				return false;

			if (childCountsPerDepth == null || childCountsPerDepth.Length == 0)
				childCountsPerDepth = new int[] { 7, 6, 5, 5 }; // Random choice: 7 in the first depth, 7 in the second etc. (excluding the root node)

			int uniqueID = 0;

			// Not setting the parrent of the root node, since it has none
			_Params.HierarchyRootNode = CreateRandomNodeModel(ref uniqueID, 0, true, childCountsPerDepth[0], childCountsPerDepth);
			_Params.FlattenedVisibleHierarchy = new List<IHierarchyNodeModel>(_Params.HierarchyRootNode.Children);
			ResetItems(_Params.FlattenedVisibleHierarchy.Count);

			return true;
		}
		public void CollapseAll()
		{
			if (_BusyWithAnimation)
				return;

			for (int i = 0; i < _Params.FlattenedVisibleHierarchy.Count;)
			{
				var m = _Params.FlattenedVisibleHierarchy[i];
				m.Expanded = false;
				if (m.Depth > 1)
				{
					_Params.FlattenedVisibleHierarchy.RemoveAt(i);
				}
				else
					++i;
			}
			ResetItems(_Params.FlattenedVisibleHierarchy.Count);
		}
		public void ExpandAll()
		{
			if (_BusyWithAnimation)
				return;

			_Params.FlattenedVisibleHierarchy = (_Params.HierarchyRootNode as FileSystemEntryModel).GetFlattenedHierarchyAndExpandAll();
			ResetItems(_Params.FlattenedVisibleHierarchy.Count);
		}
		#endregion

		/// <summary>
		/// Needed for external usage of the hierarchy features. For example, if you want to expand/collapse a node from another 
		/// script without the need to know which explicit implementation of the hierarchy is used.
		/// Returns whether the toggle could be done. For example, any running animations would prevent this
		/// </summary>
		bool IHierarchyOSA.ToggleDirectoryFoldout(int itemIndex)
		{
			var vh = GetItemViewsHolderIfVisible(itemIndex);
			return ToggleDirectoryFoldoutInternal(itemIndex, vh);
		}

		bool ToggleDirectoryFoldoutInternal(int itemIndex, IHierarchyNodeViewsHolder vhIfVisible)
		{
			if (_BusyWithAnimation)
				return false;

			var model = _Params.FlattenedVisibleHierarchy[itemIndex];
			if (!model.IsDirectory())
				return false;

			int nextIndex = itemIndex + 1;
			bool wasExpanded = model.Expanded;
			model.Expanded = !wasExpanded;
			if (wasExpanded)
			{
				// Remove all following models with bigger depth, until a model with a less than- or equal depth is found
				int i = itemIndex + 1;
				int count = _Params.FlattenedVisibleHierarchy.Count;
				for (; i < count;)
				{
					var m = _Params.FlattenedVisibleHierarchy[i];
					if (m.Depth > model.Depth)
					{
						m.Expanded = false;
						++i;
						continue;
					}

					break; // found with depth less than- or equal to the collapsed item
				}

				int countToRemove = i - nextIndex;
				if (countToRemove > 0)
				{
					if (_Params.animatedFoldOut)
						GradualRemove(nextIndex, countToRemove);
					else
					{
						_Params.FlattenedVisibleHierarchy.RemoveRange(nextIndex, countToRemove);
						RemoveItems(nextIndex, countToRemove);
					}
				}
			}
			else
			{
				if (model.Children.Length > 0)
				{
					if (_Params.animatedFoldOut)
						GradualAdd(nextIndex, model.Children);
					else
					{
						_Params.FlattenedVisibleHierarchy.InsertRange(nextIndex, model.Children);
						InsertItems(nextIndex, model.Children.Length);
					}
				}
			}

			// Starting with v4.0, only the newly visible items are updated to keep performance at max, 
			// so we need to update the directory viewsholder manually (most notably, its arrow will change)
			//Debug.Log(model.expanded + ", " + vhIfVisible);
			if (vhIfVisible != null)
				vhIfVisible.UpdateViews(model);

			return true;
		}

		void OnDirectoryFoldOutClicked(IHierarchyNodeViewsHolder vh)
		{
			if (!ToggleDirectoryFoldoutInternal(vh.ItemIndex, vh))
				return;
		}

		void GradualAdd(int index, IHierarchyNodeModel[] children) { StartCoroutine(GradualAddOrRemove(index, children.Length, children)); }

		void GradualRemove(int index, int countToRemove) { StartCoroutine(GradualAddOrRemove(index, countToRemove, null)); }

		IEnumerator GetWaitSecondsYieldInstruction(float toWait)
		{
			if (_Params.UseUnscaledTime)
				yield return new WaitForSecondsRealtime(toWait);
			else
				yield return new WaitForSeconds(toWait);
		}

		IEnumerator GradualAddOrRemove(int index, int count, IHierarchyNodeModel[] childrenIfAdd)
		{
			_BusyWithAnimation = true;
			int curIndexInChildren = 0;
			int remainingLen = count;
			int divider = Mathf.Min(7, count);
			int maxChunkSize = count / divider;
			int curChunkSize;
			float toWait = .01f;


			if (childrenIfAdd == null)
			{
				index = index + count - 1;
				while (remainingLen > 0)
				{
					curChunkSize = Math.Min(remainingLen, maxChunkSize);

					int curStartIndex = index - curChunkSize + 1;
					for (int i = index; i >= curStartIndex; --i, --index)
						_Params.FlattenedVisibleHierarchy.RemoveAt(i);
					RemoveItems(curStartIndex, curChunkSize);
					remainingLen -= curChunkSize;

					yield return GetWaitSecondsYieldInstruction(toWait);
				}
			}
			else
			{
				while (remainingLen > 0)
				{
					curChunkSize = Math.Min(remainingLen, maxChunkSize);

					int curStartIndex = index;
					for (int i = 0; i < curChunkSize; ++i, ++index, ++curIndexInChildren)
						_Params.FlattenedVisibleHierarchy.Insert(index, childrenIfAdd[curIndexInChildren]);

					InsertItems(curStartIndex, curChunkSize);
					remainingLen -= curChunkSize;

					yield return GetWaitSecondsYieldInstruction(toWait);
				}
			}
			_BusyWithAnimation = false;
		}

		// Just a utility for generating random node models. in a real-case scenario, you'd be creating the models from your data source
		FileSystemEntryModel CreateRandomNodeModel(ref int uniqueID, int depth, bool forceDirectory, int numChildren, int[] childCountsPerDepth)
		{
			if (depth < _Params.maxHierarchyDepth)
			{
				if (forceDirectory || UnityEngine.Random.Range(0, 2) == 0)
				{
					// Create a directory for the current depth
					var dirModel = CreateNewModel(ref uniqueID, depth, true);
					dirModel.Children = new FileSystemEntryModel[numChildren];
					bool forceDirectoriesOnNextDepth = depth == 1;
					for (int i = 0; i < numChildren; ++i)
					{
						int nextDepth = depth + 1;
						int numChildrenOnNextDepth = childCountsPerDepth.Length <= nextDepth ? 1 : childCountsPerDepth[nextDepth];
						var child = CreateRandomNodeModel(ref uniqueID, nextDepth, forceDirectoriesOnNextDepth, numChildrenOnNextDepth, childCountsPerDepth);
						child.Parent = dirModel;
						dirModel.Children[i] = child;
					}

					return dirModel;
				}
			}

			return CreateNewModel(ref uniqueID, depth, false);
		}

		FileSystemEntryModel CreateNewModel(ref int itemIndex, int depth, bool isDirectory)
		{
			return new FileSystemEntryModel()
			{
				Title = (isDirectory ? "Directory " : "File ") + (itemIndex++),
				Depth = depth
			};
		}
	}


	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab, IHierarchyParams
	{
		[Range(0, 10)]
		public int maxHierarchyDepth;
		public bool animatedFoldOut = true;
		public IHierarchyNodeModel HierarchyRootNode { get; set; }
		public List<IHierarchyNodeModel> FlattenedVisibleHierarchy { get; set; }
	}


	// File system entry (file or directory)
	public class FileSystemEntryModel : IHierarchyNodeModel
	{
		// The parent field is very useful to cache the parent here. For example, when you need to collapse an item's directory, 
		// you'd normally to iterate over all the previous items until one with a smaller depth is found, which is expensive
		public IHierarchyNodeModel Parent { get; set; }
		public IHierarchyNodeModel[] Children { get; set; }
		public int Depth { get; set; }
		public string Title { get; set; }
		public bool Expanded { get; set; }

		public bool IsDirectory() { return Children != null; } 


		public List<IHierarchyNodeModel> GetFlattenedHierarchyAndExpandAll()
		{
			var res = new List<IHierarchyNodeModel>();
			for (int i = 0; i < Children.Length; i++)
			{
				var c = Children[i];
				res.Add(c);
				c.Expanded = true;
				if (c.IsDirectory())
				{
					res.AddRange((c as FileSystemEntryModel).GetFlattenedHierarchyAndExpandAll());
				}
			}

			return res;
		}
	}


	public class FileSystemEntryViewsHolder : BaseItemViewsHolder, IHierarchyNodeViewsHolder
	{
		public Text titleText;
		public Image foldoutArrowImage;

		public bool CanFoldout { get { return _FoldoutButton.interactable; } set { _FoldoutButton.interactable = value; } }

		Button _FoldoutButton;
		Image _FileIconImage, _DirectoryIconImage;
		RectTransform _PanelRT;
		HorizontalLayoutGroup _RootLayoutGroup;

		//RectTransform IHierarchyNodeViewsHolder.Root { get { return root; } }


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			_RootLayoutGroup = root.GetComponent<HorizontalLayoutGroup>();
			_PanelRT = root.GetChild(0) as RectTransform;
			_PanelRT.GetComponentAtPath("TitleText", out titleText);
			_PanelRT.GetComponentAtPath("FoldOutButton", out _FoldoutButton);
			_PanelRT.GetComponentAtPath("DirectoryIconImage", out _DirectoryIconImage);
			_PanelRT.GetComponentAtPath("FileIconImage", out _FileIconImage);
			_FoldoutButton.transform.GetComponentAtPath("FoldOutArrowImage", out foldoutArrowImage);
		}

		public override void MarkForRebuild()
		{
			base.MarkForRebuild();
			LayoutRebuilder.MarkLayoutForRebuild(_PanelRT);
		}

		public void UpdateViews(IHierarchyNodeModel model)
		{
			FileSystemEntryModel modelCasted = model as FileSystemEntryModel;

			titleText.text = modelCasted.Title;
			bool isDir = modelCasted.IsDirectory();
			_FoldoutButton.interactable = isDir;
			_DirectoryIconImage.gameObject.SetActive(isDir);
			_FileIconImage.gameObject.SetActive(!isDir);
			foldoutArrowImage.gameObject.SetActive(isDir);
			if (isDir)
				foldoutArrowImage.rectTransform.localRotation = Quaternion.Euler(0, 0, modelCasted.Expanded ? -90 : 0);

			_RootLayoutGroup.padding.left = 25 * modelCasted.Depth; // the higher the depth, the higher the padding
		}

		public void SetOnToggleFoldoutListener(Action<IHierarchyNodeViewsHolder> onFouldOutToggled)
		{
			_FoldoutButton.onClick.RemoveAllListeners();
			if (onFouldOutToggled != null)
				_FoldoutButton.onClick.AddListener(() => onFouldOutToggled(this));
		}
	}
}
