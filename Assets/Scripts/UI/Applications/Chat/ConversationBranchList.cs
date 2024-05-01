#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Chat;
using FuzzySharp.Extractor;
using GameplaySystems.Chat;
using UnityEngine;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public sealed class ConversationBranchList : MonoBehaviour
	{
		[SerializeField]
		private RectTransform fewItemsList = null!;
		
		[SerializeField]
		private ConversationBranchItemView itemPrefab = null!;
		
		private readonly List<IBranchDefinition> currentList = new();
		private readonly List<ConversationBranchItemView> views = new();
		private readonly int maxViewsUntilScroll = 3;

		private int minimumScore = 50;
		private string filterQuery = string.Empty;
		private CanvasGroup canvasGroup = null!;
		private LTDescr? hideTween;
		private LTDescr? showTween;
		private readonly List<int> filteredItems = new();

		private int selected = -1;
		
		public bool IsEmpty => currentList.Count == 0;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ConversationBranchList));
			this.MustGetComponent(out canvasGroup);

			canvasGroup.alpha = 0;
			AfterHide();
		}

		public bool PickSelectedIfAny()
		{
			if (selected == -1)
				return false;

			this.PickBranch(currentList[filteredItems[selected]]);
			return true;
		}
		
		public void UpdateList(BranchDefinitionList list)
		{
			filterQuery = string.Empty;
			currentList.Clear();
			currentList.AddRange(list);

			this.UpdateLIstUI();
		}

		private void FilterItems()
		{
			filteredItems.Clear();

			if (string.IsNullOrWhiteSpace(filterQuery))
			{
				// No filter query, so order alphabetically by character name then by message.
				
				var i = 0;
				foreach (IBranchDefinition branch in currentList.OrderBy(x => x.Target.ChatName).ThenBy(x => x.Message))
				{
					filteredItems.Add(i);
					i++;
				}

				return;
			}

			foreach (ExtractedResult<string>? uwu in FuzzySharp.Process.ExtractSorted(filterQuery, this.currentList.Select(x => x.Message)))
			{
				if (uwu.Score < minimumScore)
					continue;
				
				this.filteredItems.Add(uwu.Index);
			}
		}

		private IEnumerable<(int rank, int index)> RankDistances(out int lowest)
		{
			var ranks = new (int, int)[currentList.Count];
			var lowestRank = int.MaxValue;
			for (var i = 0; i < currentList.Count; i++)
			{
				ranks[i] = (GetStringDistance(filterQuery, currentList[i].Message.Substring(0, Math.Min(filterQuery.Length, currentList[i].Message.Length))), i);
				lowestRank = Math.Min(lowestRank, ranks[i].Item1);
			}

			lowest = lowestRank;
			return ranks;
		}

		private void UpdateLIstUI()
		{
			FilterItems();

			if (filteredItems.Count == 0)
				selected = -1;
			else
				selected = Math.Clamp(selected, 0, filteredItems.Count - 1);
			
			bool isEmpty = filteredItems.Count == 0;
			bool isFew = filteredItems.Count <= maxViewsUntilScroll;

			if (isEmpty)
				Hide();
			
			fewItemsList.gameObject.SetActive(isFew && !isEmpty);

			if (isFew)
			{
				while (views.Count > filteredItems.Count)
				{
					Destroy(views[^1].gameObject);
					views.RemoveAt(views.Count-1);
				}

				for (var i = 0; i < filteredItems.Count; i++)
				{
					ConversationBranchItemView? view = null;
					if (i == views.Count)
					{
						view = Instantiate(itemPrefab, this.fewItemsList);
						views.Add(view);
					}
					else
					{
						view = views[i];
					}

					view.ClickHandler = PickBranch;
					view.UpdateView(currentList[filteredItems[i]]);
				}
			}
			else
			{
				if (views.Count > 0)
				{
					for (var i = 0; i < views.Count; i++)
						Destroy(views[i].gameObject);
					
					views.Clear();
				}
			}
		}

		private void PickBranch(IBranchDefinition branch)
		{
			// We do this so the player can actually fucking SEE what they're about to send.
			currentList.Clear();
			this.UpdateLIstUI();
			
			branch.Pick();
		}

		public void Show()
		{
			if (hideTween != null)
				LeanTween.cancel(hideTween.id);
			
			hideTween = null;

			if (showTween != null)
				return;
			
			showTween = LeanTween.alphaCanvas(canvasGroup, 1, 0.2f)
				.setOnComplete(AfterShow);
		}

		private void AfterShow()
		{
			showTween = null;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}
		
		public void Hide()
		{
			if (showTween != null)
				LeanTween.cancel(showTween.id);
			
			showTween = null;

			if (hideTween != null)
				return;
			
			hideTween = LeanTween.alphaCanvas(canvasGroup, 0, 0.2f)
				.setOnComplete(AfterHide);
		}

		public void UpdateQuery(string newQuery)
		{
			this.filterQuery = newQuery;
			this.UpdateLIstUI();

			if (this.filteredItems.Count != 0)
				Show();
		}
		
		private void AfterHide()
		{
			hideTween = null;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}
		
		/// <summary>
		///     Calculate the difference between 2 strings using the Levenshtein distance algorithm
		/// </summary>
		/// <param name="source1">First string</param>
		/// <param name="source2">Second string</param>
		/// <returns></returns>
		/// <stolenfrom>https://gist.github.com/Davidblkx/e12ab0bb2aff7fd8072632b396538560</stolenfrom>
		public static int GetStringDistance(string source1, string source2) //O(n*m)
		{
			int source1Length = source1.Length;
			int source2Length = source2.Length;

			var matrix = new int[source1Length + 1, source2Length + 1];

			// First calculation, if one entry is empty return full length
			if (source1Length == 0)
				return source2Length;

			if (source2Length == 0)
				return source1Length;

			// Initialization of matrix with row size source1Length and columns size source2Length
			for (var i = 0; i <= source1Length; matrix[i, 0] = i++){}
			for (var j = 0; j <= source2Length; matrix[0, j] = j++){}

			// Calculate rows and collumns distances
			for (var i = 1; i <= source1Length; i++)
			{
				for (var j = 1; j <= source2Length; j++)
				{
					int cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

					matrix[i, j] = Math.Min(
						Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
						matrix[i - 1, j - 1] + cost);
				}
			}
			// return result
			return matrix[source1Length, source2Length];
		}
	}
}