using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.CustomAdapters.GridView.Specialized.GridWithCategories;

namespace Com.TheFallenGames.OSA.Demos.GridWithCategories
{
    public class GridWithCategoriesDemoDataUtil
	{
		public static List<CategoryModel> CreateRandomCategories(int numCategories, int maxCountInCategory)
		{
			var categories = new List<CategoryModel>(numCategories);
			int currentItemID = 0;
			for (int i = 0; i < numCategories; i++)
			{
				int numItemsInCategory = UnityEngine.Random.Range(1, maxCountInCategory + 2);
				var cat = CreateRandomCategoryModel(currentItemID, numItemsInCategory);
				currentItemID += numItemsInCategory;
				categories.Add(cat);
			}

			return categories;
		}

		static CategoryModel CreateRandomCategoryModel(int itemsStartingID, int numItemsInCategory)
		{
			var cat = new CategoryModel();
			cat.items = new List<CellModel>(numItemsInCategory);
			cat.name = DemosUtil.GetRandomTextBody(5, 100);

			for (int i = 0; i < numItemsInCategory; i++)
			{
				var m = CreateRandomValidItemModel(cat, itemsStartingID + i);
				cat.items.Add(m);
			}

			return cat;
		}

		static CellModel CreateRandomValidItemModel(CategoryModel parentCategory, int id)
		{
			return new CellModel
			{
				ParentCategory = parentCategory,
				Id = id,
				Type = CellType.VALID
			};
		}
	}
}
