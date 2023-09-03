using System;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.CustomAdapters.GridView.Specialized.GridWithCategories
{
    public class GridWithCategoriesUtil
	{
		static T CreateItemModelForRowCompletion<T>(ICategoryModel parentCategory) where T : ICellModel, new()
		{
			return new T
			{
				ParentCategory = parentCategory,
				Id = -1,
				Type = CellType.FOR_ROW_COMPLETION
			};
		}

		static T CreateItemModelInRowSeparatingCategories<T>(ICategoryModel parentCategory) where T : ICellModel, new()
		{
			return new T
			{
				ParentCategory = parentCategory,
				Id = -1,
				Type = CellType.IN_ROW_SEPARATING_CATEGORIES
			};
		}

		/// <summary>
		/// Converts your user-level data structure to an OSA Grid-compatible list of items, which will also include "filler" items for the spaces between the categories and empty spaces.
		/// This approach has the advantage of reusing the OSA's GridAdapter completely, at the expense of creating/managing few additional items at the user-level
		/// </summary>
		public static void ConvertCategoriesToListOfItemModels<TCategory, TCell>(int itemSlotsPerRow, List<TCategory> categories, out List<TCell> cells) 
			where TCategory : ICategoryModel
			where TCell : ICellModel, new()
		{
			cells = new List<TCell>();
			for (int i = 0; i < categories.Count; i++)
			{
				var cat = categories[i];

				// Insert an empty row of items to make room for the category's header
				for (int j = 0; j < itemSlotsPerRow; j++)
				{
					var m = CreateItemModelInRowSeparatingCategories<TCell>(cat);
					cells.Add(m);
				}

				// Add the actual cells
				for (int j = 0; j < cat.Count; j++)
					cells.Add((TCell)cat[j]);

				// If the category's last row is not full, fill it with empty slots, so they won't be occupied with the ones from the next category
				while (cells.Count % itemSlotsPerRow != 0)
					cells.Add(CreateItemModelForRowCompletion<TCell>(cat));
			}
		}
	}
}
