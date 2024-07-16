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
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ConversationBranchList));
			this.MustGetComponent(out canvasGroup);

			canvasGroup.alpha = 0;
			AfterHide();
		}
		
		public void UpdateList(BranchDefinitionList list)
		{
			filterQuery = string.Empty;
			currentList.Clear();
			currentList.AddRange(list);

			this.UpdateLIstUI();
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